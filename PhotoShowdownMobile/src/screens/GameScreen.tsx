import React, { useEffect, useState, useRef } from 'react';
import { 
  View, Text, StyleSheet, ActivityIndicator, ScrollView, Image, 
  TouchableOpacity, Alert, Modal, SafeAreaView, Dimensions
} from 'react-native';

import { socketService } from '../utils/WebSocketService';
import matchesService from '../services/matchesService';
import picturesService from '../services/picturesService';
import { GameState, parseRoundState, parseMatchState } from '../enums/GameState';
import { IP_ADDRESS, PORT } from '../config'; 

const GameScreen = ({ route, navigation }: any) => {
  const { matchId, token, userId, username } = route.params;
  
  const [status, setStatus] = useState("Loading...");
  const [roundData, setRoundData] = useState<any>(null);
  const [matchPlayers, setMatchPlayers] = useState<any[]>([]);
  const [myPictures, setMyPictures] = useState<any[]>([]);
  
  const [totalRounds, setTotalRounds] = useState<number>(0); 
  
  // Offset to sync client clock with server clock
  const [timeOffset, setTimeOffset] = useState<number>(0);

  // --- Selection & Voting State ---
  const [hasSelected, setHasSelected] = useState(false); 
  const [tempSelectedPictureId, setTempSelectedPictureId] = useState<number | null>(null);
  const [tempVotedPictureId, setTempVotedPictureId] = useState<number | null>(null);

  const [secondsLeft, setSecondsLeft] = useState(0);
  const [zoomedImage, setZoomedImage] = useState<string | null>(null);
  const [showLeaderboard, setShowLeaderboard] = useState(false);
  const [isMatchOver, setIsMatchOver] = useState(false);

  const [processedRoundIndex, setProcessedRoundIndex] = useState<number>(-1);
  
  const isMatchOverRef = useRef(false);
  const prevRoundStateRef = useRef<string>('');

  const WS_URL = `ws://${IP_ADDRESS}:${PORT}/api/ws`;

 // --- Initial Setup & WebSocket ---
  useEffect(() => {
    fetchMyPictures();
    fetchCurrentState();
    
    socketService.connect(WS_URL, token);

    const unsubscribe = socketService.subscribe((msg: any) => {
      if (isMatchOverRef.current) return;

      const rawType = msg.type || msg.Type || '';
      const msgTypeLower = rawType.toString().toLowerCase();

      if (msgTypeLower === 'matchended' || msgTypeLower === 6) {
          console.log("WebSocket received MatchEnded! Switching UI.");
          
          isMatchOverRef.current = true; 
          setIsMatchOver(true);
          
          socketService.disconnect(); 
          return;
      }

      if (msg.data && msg.data.roundState !== undefined) {
          console.log("Round Update via WS:", msg.data.roundState);
          setRoundData(msg.data);
      }
    });

    return () => {
      console.log("GameScreen Unmounting");
      unsubscribe(); 
    };
  }, []);

  const currentRoundState = roundData ? parseRoundState(roundData.roundState) : '';

  // --- Logic Effects ---
  useEffect(() => {
      if (!roundData) return;

      // 1. Handle State Transitions (Reset Locks)
      if (prevRoundStateRef.current !== currentRoundState) {
          console.log(`State Changed: ${prevRoundStateRef.current} -> ${currentRoundState}`);
          
          if (currentRoundState === GameState.PictureSelection) {
              setHasSelected(false);
              setTempSelectedPictureId(null);
              setTempVotedPictureId(null);
              setStatus("Pick your card:");
          } else if (currentRoundState === GameState.Voting) {
              setHasSelected(false); 
              setTempVotedPictureId(null);
          }

          prevRoundStateRef.current = currentRoundState;
      }

      // 2. Handle Round End & Scoring
      if (currentRoundState === GameState.Ended) {
          
          if (roundData.roundIndex > processedRoundIndex) {
              
              if (roundData.roundWinnerId) {
                  console.log(`Winner ID: ${roundData.roundWinnerId}. Updating local score.`);
                  
                  setMatchPlayers(prevPlayers => {
                      return prevPlayers.map(player => {
                          if (player.id === roundData.roundWinnerId) {
                              return { ...player, score: player.score + 1 };
                          }
                          return player;
                      });
                  });
              }
              
              setProcessedRoundIndex(roundData.roundIndex);

              // Check if this was the last round
              if (totalRounds > 0 && (roundData.roundIndex + 1) >= totalRounds) {
                  console.log("Last round ended. Closing connection immediately.");
                  
                  isMatchOverRef.current = true;
                  socketService.disconnect(); // Explicit disconnect
                  setIsMatchOver(true);
              }
          }
      }
  }, [roundData, currentRoundState, totalRounds, processedRoundIndex]);

  // --- Timer with Server Offset Fix ---
  useEffect(() => {
      if (!roundData || isMatchOver) return;

      const updateTimer = () => {
          // Calculate current time ADJUSTED by the offset we calculated earlier
          const adjustedNow = Date.now() + timeOffset;
          let targetTime = 0;
          
          if (currentRoundState === GameState.PictureSelection && roundData.pictureSelectionEndDate) {
              targetTime = Date.parse(roundData.pictureSelectionEndDate);
          } else if (currentRoundState === GameState.Voting && roundData.votingEndDate) {
              targetTime = Date.parse(roundData.votingEndDate);
          } else if (currentRoundState === GameState.Ended && roundData.roundEndDate) {
              targetTime = Date.parse(roundData.roundEndDate);
          }

          if (targetTime > 0) {
              const diff = Math.floor((targetTime - adjustedNow) / 1000);
              setSecondsLeft(diff > 0 ? diff : 0);
          } else {
              setSecondsLeft(0);
          }
      };

      updateTimer();
      const interval = setInterval(updateTimer, 1000);
      return () => clearInterval(interval);
  }, [roundData, currentRoundState, isMatchOver, timeOffset]);

  // --- API Calls ---

  const fetchCurrentState = async () => {
      if (isMatchOverRef.current) return;

      try {
          const res = await matchesService.getCurrentMatch(token);
          
          // Calculate Clock Skew
          if (res.headers && res.headers['date']) {
              const serverTime = Date.parse(res.headers['date']);
              const clientTime = Date.now();
              const offset = serverTime - clientTime;
              console.log(`Clock Sync: Client is ${offset}ms off from Server`);
              setTimeOffset(offset);
          }

          if (res.data.data) {
              const data = res.data.data;
              
              if (data.users) {
                  console.log("👥 Syncing players & scores from server:", data.users);
                  setMatchPlayers(data.users);
              }
              // ------------------

              const rounds = 
                  data.numOfRounds ?? 
                  data.NumOfRounds ?? 
                  (data.match && data.match.numOfRounds) ?? 
                  (data.match && data.match.NumOfRounds) ?? 
                  0;

              if (rounds > 0) setTotalRounds(Number(rounds));
              
              if (data.round) {
                  setRoundData(data.round);
                  if (prevRoundStateRef.current === '') {
                        prevRoundStateRef.current = parseRoundState(data.round.roundState);
                  }
                  
                  if (data.round.roundState === 2) { 
                      setProcessedRoundIndex(data.round.roundIndex);
                  }
              }

              const matchState = parseMatchState(data.MatchState !== undefined ? data.MatchState : data.matchState);
              if (matchState === GameState.Ended) {
                  isMatchOverRef.current = true;
                  socketService.disconnect();
                  setIsMatchOver(true);
              }
          }
      } catch (error) { console.error("Fetch State Error:", error); }
  };

  const fetchMyPictures = async () => {
      try {
          const res = await picturesService.getMyPictures(token);
          if (res.data.data) setMyPictures(res.data.data);
      } catch (e) { console.log(e); }
  };

  // --- User Actions ---

  const onLeaveMatch = () => {
    Alert.alert(
        "Leave Match",
        "Are you sure you want to exit the game?",
        [
            { text: "Cancel", style: "cancel" },
            { 
                text: "Leave", 
                style: "destructive", 
                onPress: () => {
                    try { socketService.disconnect(); } catch(e) { console.error(e); }
                    navigation.replace('Home', { token, userId, username });
                }
            }
        ]
    );
  };

  const onPictureClick = (pictureId: number) => {
      setTempSelectedPictureId(pictureId);
  };

  const handleLockInSelection = async () => {
      if (!tempSelectedPictureId) return;
      
      setHasSelected(true);
      try {
          const payload = { pictureId: tempSelectedPictureId, matchId, roundIndex: roundData.roundIndex };
          await matchesService.selectPictureForRound(payload, token);
          setStatus("Waiting for others...");
      } catch (error: any) { 
          setHasSelected(false);
          Alert.alert("Error", "Selection failed."); 
      }
  };

  const onVoteClick = (roundPictureId: number) => {
      setTempVotedPictureId(roundPictureId);
  };

  const handleLockInVote = async () => {
      if (!tempVotedPictureId) return;

      setHasSelected(true);
      try {
          const payload = { roundPictureId: tempVotedPictureId, matchId, roundIndex: roundData.roundIndex };
          await matchesService.voteForPicture(payload, token);
          Alert.alert("Voted!", "Waiting for results...");
      } catch (error) { 
          setHasSelected(false);
          Alert.alert("Error", "Vote failed"); 
      }
  };

  // --- Helpers ---
  const getImageUrl = (path: string) => {
      if (!path) return undefined;
      let cleanPath = path.replace(/\\/g, '/');
      if (cleanPath.startsWith('/')) cleanPath = cleanPath.substring(1);
      if (!cleanPath.startsWith('pictures/')) cleanPath = `pictures/${cleanPath}`;
      return `http://${IP_ADDRESS}:${PORT}/${cleanPath}`;
  };

  const getWinnerDetails = () => {
      if (!roundData || !roundData.roundWinnerId) return null;
      const winner = matchPlayers.find(p => p.id === roundData.roundWinnerId);
      const winningPic = roundData.picturesSelected?.find((p: any) => p.selectedByUserId === roundData.roundWinnerId);

      return {
          name: winner ? winner.username : "Unknown Player",
          picUrl: winningPic ? getImageUrl(winningPic.picturePath) : null
      };
  };

  const goBackHome = () => {
      try { socketService.disconnect(); } catch (e) { console.error(e); }
      navigation.replace('Home', { token, userId, username });
  };

  // --- Render Components ---

  const renderLeaderboard = (isFinal: boolean = false) => (
    <View style={[styles.leaderboard, isFinal && styles.finalLeaderboard]}>
        <Text style={styles.leaderboardTitle}>
            {isFinal ? "🏆 Final Standings 🏆" : "Leaderboard"}
        </Text>
        {matchPlayers
            .sort((a, b) => b.score - a.score)
            .map((player, index) => (
            <View key={player.id} style={styles.scoreRow}>
                <Text style={[styles.scoreName, isFinal && index === 0 && styles.winnerName]}>
                    {isFinal && index === 0 ? "👑 " : `${index + 1}. `}
                    {player.username} {player.id === userId ? "(You)" : ""}
                </Text>
                <Text style={styles.scoreValue}>{player.score.toFixed(0)}</Text>
            </View>
        ))}
    </View>
  );

  // --- Game Over View ---
  if (isMatchOver) {
      const sortedPlayers = [...matchPlayers].sort((a, b) => b.score - a.score);
      const winner = sortedPlayers[0];
      const isWinnerMe = winner?.id === userId;

      return (
          <SafeAreaView style={styles.gameOverContainer}>
              <ScrollView contentContainerStyle={styles.gameOverScroll}>
                  <Text style={styles.gameOverTitle}>GAME OVER</Text>
                  
                  <View style={styles.winnerSection}>
                      <View style={styles.winnerAvatarContainer}>
                          <Text style={styles.winnerAvatarEmoji}>{isWinnerMe ? "😎" : "👑"}</Text>
                      </View>
                      <Text style={styles.winnerLabel}>THE WINNER IS</Text>
                      <Text style={styles.winnerNameLarge}>{winner?.username || "Unknown"}</Text>
                      <Text style={styles.winnerScoreLarge}>{winner?.score.toFixed(0)} pts</Text>
                  </View>

                  <View style={styles.finalLeaderboardContainer}>
                      <Text style={styles.finalLeaderboardTitle}>Final Standings</Text>
                      {sortedPlayers.map((player, index) => (
                          <View key={player.id} style={[styles.finalRow, player.id === userId && styles.myFinalRow]}>
                              <View style={styles.rankBadge}>
                                  <Text style={styles.rankText}>#{index + 1}</Text>
                              </View>
                              <Text style={[styles.finalRowName, player.id === userId && styles.myFinalRowText]}>
                                  {player.username} {player.id === userId ? "(You)" : ""}
                              </Text>
                              <Text style={styles.finalRowScore}>{player.score.toFixed(0)}</Text>
                          </View>
                      ))}
                  </View>

                  <TouchableOpacity style={styles.homeButton} onPress={goBackHome}>
                      <Text style={styles.homeButtonText}>🏠 Back to Main Menu</Text>
                  </TouchableOpacity>
              </ScrollView>
          </SafeAreaView>
      );
  }

  // --- Loading View ---
  if (!roundData) {
      return (
          <View style={styles.container}>
              <ActivityIndicator size="large" color="#03DAC6" />
              <Text style={styles.text}>Syncing Game...</Text>
          </View>
      );
  }

  // --- Main Game View ---
  return (
    <View style={styles.container}>
      
      <Modal visible={!!zoomedImage} transparent={true} animationType="fade">
          <View style={styles.modalBackground}>
              {zoomedImage && (
                  <Image source={{ uri: zoomedImage }} style={styles.fullImage} resizeMode="contain" />
              )}
          </View>
      </Modal>

      <Modal visible={showLeaderboard} transparent={true} animationType="slide">
          <View style={styles.modalBackground}>
              <View style={styles.modalContent}>
                  {renderLeaderboard()}
                  <TouchableOpacity style={styles.closeBtn} onPress={() => setShowLeaderboard(false)}>
                      <Text style={styles.closeBtnText}>Close</Text>
                  </TouchableOpacity>
              </View>
          </View>
      </Modal>

      {currentRoundState !== GameState.Ended && (
        <>
            <TouchableOpacity style={styles.leaveBtn} onPress={onLeaveMatch}>
                <Text style={styles.leaveBtnText}>🚪 Leave</Text>
            </TouchableOpacity>

            <TouchableOpacity style={styles.trophyBtn} onPress={() => setShowLeaderboard(true)}>
                <Text style={{fontSize: 24}}>🏆</Text>
            </TouchableOpacity>
        </>
      )}

      {currentRoundState !== GameState.Ended && (
        <View style={styles.timerContainer}>
            <Text style={styles.timerLabel}>Time Left</Text>
            <Text style={[styles.timerValue, secondsLeft < 10 && styles.timerUrgent]}>{secondsLeft}s</Text>
        </View>
      )}

      {/* Round Info Display */}
      {currentRoundState !== GameState.Ended && (
        <View style={styles.roundInfoContainer}>
             <Text style={styles.roundInfoText}>
                Round {roundData.roundIndex !== undefined ? roundData.roundIndex + 1 : 1}
             </Text>
        </View>
      )}

      {currentRoundState !== GameState.Ended && (
        <View style={styles.sentenceCard}>
            <Text style={styles.sentenceText}>{roundData.sentence}</Text>
        </View>
      )}

      {/* State: Selection */}
      {currentRoundState === GameState.PictureSelection && (
          <>
            <Text style={styles.sectionTitle}>Pick your card (Hold to zoom):</Text>
            {hasSelected && (
                <Text style={{color: '#4CAF50', marginBottom: 10, fontWeight:'bold'}}>✅ Choice Locked In</Text>
            )}
            
            <ScrollView 
                style={styles.cardsScroll} 
                contentContainerStyle={styles.cardsGrid}
            >
                <View style={styles.row}>
                    {myPictures.map((pic) => (
                        <TouchableOpacity 
                            key={pic.id} 
                            onPress={() => {
                                if (!hasSelected) onPictureClick(pic.id);
                            }}
                            onLongPress={() => setZoomedImage(getImageUrl(pic.picturePath) || null)}
                            onPressOut={() => setZoomedImage(null)} // Closes zoom when finger released
                            delayLongPress={200} // Slightly faster response
                            style={[
                                styles.cardWrapper, 
                                tempSelectedPictureId === pic.id && styles.selectedCard,
                                hasSelected && tempSelectedPictureId !== pic.id && styles.disabledCard
                            ]}
                        >
                            <Image source={{ uri: getImageUrl(pic.picturePath) }} style={styles.cardImage} resizeMode="cover"/>
                        </TouchableOpacity>
                    ))}
                </View>
            </ScrollView>

            {!hasSelected && (
                <TouchableOpacity 
                    style={[styles.lockInButton, !tempSelectedPictureId && styles.disabledButton]} 
                    onPress={handleLockInSelection}
                    disabled={!tempSelectedPictureId}
                >
                    <Text style={styles.lockInButtonText}>Lock In Selection 🔒</Text>
                </TouchableOpacity>
            )}
          </>
      )}

      {/* State: Voting */}
      {currentRoundState === GameState.Voting && (
          <>
            <Text style={styles.sectionTitle}>Vote for the funniest!</Text>
            <ScrollView 
                style={styles.cardsScroll} 
                contentContainerStyle={styles.cardsGrid}
            >
                <View style={styles.row}>
                    {roundData.picturesSelected?.map((picSelected: any) => {
                        // Check if this picture belongs to the current user
                        const isMyPicture = picSelected.selectedByUserId === userId;

                        return (
                            <TouchableOpacity 
                                key={picSelected.id}
                                onPress={() => {
                                    if (!hasSelected && !isMyPicture) onVoteClick(picSelected.id);
                                }}
                                onLongPress={() => setZoomedImage(getImageUrl(picSelected.picturePath) || null)}
                                onPressOut={() => setZoomedImage(null)} // Closes zoom when finger released
                                delayLongPress={200}
                                style={[
                                    styles.cardWrapper, 
                                    tempVotedPictureId === picSelected.id && styles.selectedCard,
                                    (hasSelected && tempVotedPictureId !== picSelected.id) && styles.disabledCard,
                                    isMyPicture && styles.disabledCard // Grey out own picture
                                ]}
                            >
                                <Image source={{ uri: getImageUrl(picSelected.picturePath) }} style={styles.cardImage} resizeMode="cover" />
                                {isMyPicture && (
                                    <View style={styles.myCardOverlay}>
                                        <Text style={styles.myCardText}>Your Card</Text>
                                    </View>
                                )}
                            </TouchableOpacity>
                        );
                    })}
                </View>
            </ScrollView>

            {!hasSelected && (
                <TouchableOpacity 
                    style={[styles.lockInButton, !tempVotedPictureId && styles.disabledButton]} 
                    onPress={handleLockInVote}
                    disabled={!tempVotedPictureId}
                >
                    <Text style={styles.lockInButtonText}>Lock In Vote 🗳️</Text>
                </TouchableOpacity>
            )}
          </>
      )}

      {/* State: Round Ended */}
      {currentRoundState === GameState.Ended && (
          <ScrollView contentContainerStyle={styles.scrollContainer}>
            <Text style={styles.sectionTitle}>🏆 Round Results 🏆</Text>
            <Text style={styles.roundInfoText}>
                Round {roundData.roundIndex !== undefined ? roundData.roundIndex + 1 : 1}
            </Text>
            
            {getWinnerDetails()?.picUrl ? (
                <View style={styles.winnerContainer}>
                    <Image source={{ uri: getWinnerDetails()?.picUrl! }} style={styles.winnerImage} resizeMode="contain" />
                    <Text style={styles.winnerText}>{getWinnerDetails()?.name} Wins!</Text>
                </View>
            ) : (
                <View style={styles.winnerContainer}>
                    <Text style={{color: 'gray', fontSize: 18}}>Tie / No Votes</Text>
                </View>
            )}

            <Text style={styles.subText}>Next round in {secondsLeft}s...</Text>
            {renderLeaderboard(false)}
          </ScrollView>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#121212', padding: 20, paddingTop: 40, alignItems: 'center' },
  scrollContainer: { flexGrow: 1, width: '100%', alignItems: 'center' },
  text: { color: 'white', marginTop: 20, fontSize: 18 },
  
  modalBackground: { flex: 1, backgroundColor: 'rgba(0,0,0,0.9)', justifyContent: 'center', alignItems: 'center' },
  modalCloseArea: { position: 'absolute', top: 0, bottom: 0, left: 0, right: 0 },
  modalContent: { width: '90%', backgroundColor: '#222', borderRadius: 15, padding: 20, alignItems: 'center' },
  fullImage: { width: '100%', height: '80%' },
  closeBtn: { marginTop: 20, backgroundColor: '#FF5252', paddingVertical: 10, paddingHorizontal: 20, borderRadius: 20 },
  closeBtnText: { color: 'white', fontWeight: 'bold', fontSize: 16 },

  leaveBtn: { position: 'absolute', top: 40, left: 20, paddingVertical: 8, paddingHorizontal: 12, backgroundColor: '#333', borderRadius: 25, zIndex: 10, borderWidth: 1, borderColor: '#555' },
  leaveBtnText: { color: '#FF5252', fontWeight: 'bold' },

  trophyBtn: { position: 'absolute', top: 90, left: 20, padding: 10, backgroundColor: '#333', borderRadius: 25, zIndex: 10, borderWidth: 1, borderColor: '#555' },
  
  timerContainer: { position: 'absolute', top: 40, right: 20, backgroundColor: '#333', padding: 8, borderRadius: 8, alignItems: 'center', borderWidth: 1, borderColor: '#555', zIndex: 10 },
  timerLabel: { color: '#aaa', fontSize: 10, textTransform: 'uppercase' },
  timerValue: { color: 'white', fontSize: 22, fontWeight: 'bold' },
  timerUrgent: { color: '#FF5252' },

  roundInfoContainer: { marginTop: 40, marginBottom: 10, alignItems: 'center' },
  roundInfoText: { color: '#AAA', fontSize: 16, fontWeight: 'bold', textTransform: 'uppercase', letterSpacing: 1 },

  winnerContainer: { alignItems: 'center', marginVertical: 20, width: '100%' },
  winnerImage: { width: 250, height: 250, borderRadius: 10, marginBottom: 15, borderWidth: 3, borderColor: '#FFD700' },
  winnerText: { color: '#FFD700', fontSize: 24, fontWeight: 'bold' },
  subText: { color: '#aaa', marginBottom: 20 },

  leaderboard: { width: '100%', backgroundColor: '#1E1E1E', borderRadius: 15, padding: 15, marginBottom: 20 },
  finalLeaderboard: { marginTop: 30, backgroundColor: '#222', borderWidth: 1, borderColor: '#444' },
  leaderboardTitle: { color: 'white', fontSize: 18, fontWeight: 'bold', marginBottom: 10, borderBottomWidth: 1, borderBottomColor: '#333', paddingBottom: 5, textAlign: 'center' },
  scoreRow: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: 8 },
  scoreName: { color: 'white', fontSize: 16 },
  winnerName: { color: '#FFD700', fontWeight: 'bold', fontSize: 18 },
  scoreValue: { color: '#03DAC6', fontSize: 16, fontWeight: 'bold' },

  sentenceCard: { backgroundColor: '#1E1E1E', padding: 20, borderRadius: 20, width: '100%', minHeight: 120, justifyContent: 'center', alignItems: 'center', marginTop: 10, marginBottom: 20, borderWidth: 1, borderColor: '#333' },
  sentenceText: { color: '#fff', fontSize: 22, fontWeight: 'bold', textAlign: 'center' },
  sectionTitle: { color: '#03DAC6', fontSize: 20, marginBottom: 15, fontWeight: 'bold', alignSelf: 'flex-start' },
  
  // Updated Styles for Layout Fix
  cardsScroll: { width: '100%' },
  cardsGrid: { paddingBottom: 100 }, 
  
  row: { 
    width: '100%',
    flexDirection: 'row', 
    flexWrap: 'wrap', 
    justifyContent: 'space-between',
  },
  cardWrapper: { 
    width: '48%', 
    height: 160,
    borderRadius: 10, 
    overflow: 'hidden', 
    borderWidth: 2, 
    borderColor: 'transparent', 
    marginBottom: 15,
  },
  selectedCard: { borderColor: '#03DAC6', transform: [{ scale: 1.05 }] },
  disabledCard: { opacity: 0.5 },
  cardImage: { 
    width: '100%', 
    height: '100%',
    backgroundColor: '#333' 
  },
  myCardOverlay: { ...StyleSheet.absoluteFillObject, backgroundColor: 'rgba(0,0,0,0.6)', justifyContent: 'center', alignItems: 'center' },
  myCardText: { color: 'white', fontWeight: 'bold', fontSize: 12 },

  lockInButton: { width: '100%', backgroundColor: '#03DAC6', padding: 15, borderRadius: 30, alignItems: 'center', marginTop: 10, marginBottom: 40 }, 
  disabledButton: { backgroundColor: '#333', opacity: 0.6 },
  lockInButtonText: { color: '#000', fontWeight: 'bold', fontSize: 18 },

  gameOverContainer: { flex: 1, backgroundColor: '#0f0f13' }, 
  gameOverScroll: { flexGrow: 1, alignItems: 'center', padding: 20, paddingTop: 40 },
  gameOverTitle: { fontSize: 42, fontWeight: '900', color: '#FF5252', letterSpacing: 2, textShadowColor: 'rgba(255, 82, 82, 0.5)', textShadowOffset: { width: 0, height: 0 }, textShadowRadius: 10, marginBottom: 30 },
  winnerSection: { alignItems: 'center', marginBottom: 40 },
  winnerAvatarContainer: { width: 100, height: 100, borderRadius: 50, backgroundColor: '#2A2A35', justifyContent: 'center', alignItems: 'center', borderWidth: 3, borderColor: '#FFD700', marginBottom: 15, elevation: 10 },
  winnerAvatarEmoji: { fontSize: 50 },
  winnerLabel: { color: '#888', fontSize: 14, letterSpacing: 1, marginBottom: 5 },
  winnerNameLarge: { color: '#FFD700', fontSize: 36, fontWeight: 'bold' },
  winnerScoreLarge: { color: '#FFF', fontSize: 22, fontWeight: '300' },
  finalLeaderboardContainer: { width: '100%', backgroundColor: '#1E1E24', borderRadius: 16, padding: 20, marginBottom: 30, borderWidth: 1, borderColor: '#333' },
  finalLeaderboardTitle: { color: 'white', fontSize: 18, fontWeight: 'bold', marginBottom: 15, textAlign: 'center', opacity: 0.8 },
  finalRow: { flexDirection: 'row', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#2A2A35' },
  myFinalRow: { backgroundColor: 'rgba(3, 218, 198, 0.1)', borderRadius: 8, paddingHorizontal: 10, marginHorizontal: -10 },
  rankBadge: { width: 30, height: 30, borderRadius: 15, backgroundColor: '#333', justifyContent: 'center', alignItems: 'center', marginRight: 15 },
  rankText: { color: '#FFF', fontWeight: 'bold', fontSize: 12 },
  finalRowName: { flex: 1, color: '#DDD', fontSize: 16 },
  myFinalRowText: { color: '#03DAC6', fontWeight: 'bold' },
  finalRowScore: { color: '#FFD700', fontSize: 18, fontWeight: 'bold' },
  homeButton: { backgroundColor: '#6200EE', paddingVertical: 18, paddingHorizontal: 50, borderRadius: 30, width: '100%', alignItems: 'center', elevation: 5 },
  homeButtonText: { color: 'white', fontSize: 18, fontWeight: 'bold', letterSpacing: 0.5 }
});

export default GameScreen;