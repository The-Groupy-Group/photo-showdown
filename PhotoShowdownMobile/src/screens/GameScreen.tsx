import React, { useEffect, useState, useRef } from 'react';
import { 
  View, Text, StyleSheet, ActivityIndicator, ScrollView, Image, 
  TouchableOpacity, Alert, Modal, Dimensions 
} from 'react-native';
import WebSocketService from '../utils/WebSocketService';
import axios from 'axios';
import { API_BASE, WS_URL, IMAGE_BASE } from '../utils/Config';

const GameScreen = ({ route }: any) => {
  const { matchId, token, userId } = route.params;
  
  const [status, setStatus] = useState("Loading...");
  const [roundData, setRoundData] = useState<any>(null);
  const [matchPlayers, setMatchPlayers] = useState<any[]>([]);
  const [myPictures, setMyPictures] = useState<any[]>([]);
  
  const [selectedPictureId, setSelectedPictureId] = useState<number | null>(null);
  const [votedPictureId, setVotedPictureId] = useState<number | null>(null);
  const [hasSelected, setHasSelected] = useState(false); 
  
  const [secondsLeft, setSecondsLeft] = useState(0);
  
  const [zoomedImage, setZoomedImage] = useState<string | null>(null);
  const [showLeaderboard, setShowLeaderboard] = useState(false);

  const isConnected = useRef(false);

  const authHeader = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    fetchMyPictures();
    fetchCurrentState();
    connectToGame();

    const unsubscribe = WebSocketService.subscribe((msg: any) => {
      console.log("Game Event:", msg);
      if (msg.data && msg.data.roundState) {
          setRoundData(msg.data);
      }
    });

    return () => {
      unsubscribe();
      WebSocketService.disconnect();
    };
  }, []);

  // --- ניהול נעילות ומעברי שלבים ---
  useEffect(() => {
      if (!roundData) return;

      const state = roundData.roundState.toLowerCase();

      // אם התחילה ההצבעה ועוד לא בחרנו למי להצביע -> משחררים את הנעילה מהשלב הקודם
      if (state === 'voting' && votedPictureId === null) {
          setHasSelected(false);
      }

      // אם התחיל סיבוב חדש לגמרי -> מאפסים הכל
      if (state === 'pictureselection' && selectedPictureId === null) {
          setHasSelected(false);
          setVotedPictureId(null);
          setStatus("Pick your card:");
      }

      // אם הסיבוב נגמר -> מרעננים ניקוד
      if (state === 'ended') {
          fetchCurrentState();
      }

  }, [roundData]); // רץ בכל פעם שמגיע עדכון מהשרת

  // --- טיימר ---
  useEffect(() => {
      if (!roundData) return;

      const interval = setInterval(() => {
          const now = new Date().getTime();
          let targetTime = 0;
          const state = roundData.roundState.toLowerCase();

          if (state === 'pictureselection') {
              targetTime = new Date(roundData.pictureSelectionEndDate).getTime();
          } else if (state === 'voting') {
              targetTime = new Date(roundData.votingEndDate).getTime();
          } else if (state === 'ended') {
              targetTime = new Date(roundData.roundEndDate).getTime();
          }

          const diff = Math.floor((targetTime - now) / 1000);
          setSecondsLeft(diff > 0 ? diff : 0);

      }, 1000);

      return () => clearInterval(interval);
  }, [roundData]);

  const fetchCurrentState = async () => {
      try {
          const res = await axios.get(`${API_BASE}/Matches/GetCurrentMatch`, authHeader);
          if (res.data.data) {
              setMatchPlayers(res.data.data.users || []);
              if (res.data.data.round) {
                  setRoundData(res.data.data.round);
              }
          }
      } catch (error) { console.error("Fetch State Error:", error); }
  };

  const fetchMyPictures = async () => {
      try {
          const res = await axios.get(`${API_BASE}/Pictures/GetMyPictures`, authHeader);
          if (res.data.data) setMyPictures(res.data.data);
      } catch (e) { console.log(e); }
  };

  const connectToGame = () => {
      if (isConnected.current) return;
      WebSocketService.connect(WS_URL, token);
      isConnected.current = true;
  };

  const handleSelectPicture = async (pictureId: number) => {
      if (hasSelected) return;

      setHasSelected(true);
      setSelectedPictureId(pictureId);

      try {
          const payload = {
              pictureId: pictureId,
              matchId: matchId,
              roundIndex: roundData.roundIndex
          };
          await axios.post(`${API_BASE}/Matches/SelectPictureForRound`, payload, authHeader);
          setStatus("Waiting for others...");
      } catch (error: any) { 
          setHasSelected(false);
          setSelectedPictureId(null);
          Alert.alert("Error", "Selection failed. Please try again."); 
      }
  };

  const handleVote = async (roundPictureId: number) => {
      if (hasSelected) return;

      setHasSelected(true);
      setVotedPictureId(roundPictureId);
      
      try {
          const payload = {
              roundPictureId: roundPictureId,
              matchId: matchId,
              roundIndex: roundData.roundIndex
          };
          await axios.post(`${API_BASE}/Matches/VoteForSelectedPicture`, payload, authHeader);
          Alert.alert("Voted!", "Waiting for results...");
      } catch (error) { 
          setHasSelected(false);
          setVotedPictureId(null);
          Alert.alert("Error", "Vote failed"); 
      }
  };

  const getImageUrl = (path: string) => {
      if (!path) return undefined;
      let cleanPath = path.replace(/\\/g, '/');
      cleanPath = cleanPath.replace('pictures/', ''); 
      return `${IMAGE_BASE}/pictures/${cleanPath}`;
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

  const renderLeaderboard = () => (
    <View style={styles.leaderboard}>
        <Text style={styles.leaderboardTitle}>Leaderboard</Text>
        {matchPlayers
            .sort((a, b) => b.score - a.score)
            .map((player) => (
            <View key={player.id} style={styles.scoreRow}>
                <Text style={styles.scoreName}>
                    {player.username} {player.id === userId ? "(You)" : ""}
                </Text>
                <Text style={styles.scoreValue}>{player.score.toFixed(1)}</Text>
            </View>
        ))}
    </View>
  );

  if (!roundData) {
      return (
          <View style={styles.container}>
              <ActivityIndicator size="large" color="#03DAC6" />
              <Text style={styles.text}>Syncing Game...</Text>
          </View>
      );
  }

  const currentState = roundData.roundState?.toLowerCase();

  return (
    <View style={styles.container}>
      
      <Modal visible={!!zoomedImage} transparent={true} animationType="fade">
          <View style={styles.modalBackground}>
              <TouchableOpacity style={styles.modalCloseArea} onPress={() => setZoomedImage(null)} />
              <View style={styles.modalContent}>
                  {zoomedImage && (
                      <Image source={{ uri: zoomedImage }} style={styles.fullImage} resizeMode="contain" />
                  )}
                  <TouchableOpacity style={styles.closeBtn} onPress={() => setZoomedImage(null)}>
                      <Text style={styles.closeBtnText}>Close</Text>
                  </TouchableOpacity>
              </View>
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

      {currentState !== 'ended' && (
        <TouchableOpacity style={styles.trophyBtn} onPress={() => setShowLeaderboard(true)}>
            <Text style={{fontSize: 24}}>🏆</Text>
        </TouchableOpacity>
      )}

      {currentState !== 'ended' && (
        <View style={styles.timerContainer}>
            <Text style={styles.timerLabel}>Time Left</Text>
            <Text style={[styles.timerValue, secondsLeft < 10 && styles.timerUrgent]}>
                {secondsLeft}s
            </Text>
        </View>
      )}

      {currentState !== 'ended' && (
        <View style={styles.sentenceCard}>
            <Text style={styles.sentenceText}>{roundData.sentence}</Text>
        </View>
      )}

      {/* מסך 1: בחירה */}
      {currentState === 'pictureselection' && (
          <>
            <Text style={styles.sectionTitle}>Pick your card (Long press to zoom):</Text>
            {status.includes("Waiting") && (
                <Text style={{color: '#4CAF50', marginBottom: 10, fontWeight:'bold'}}>✅ Choice Locked In</Text>
            )}
            
            <ScrollView contentContainerStyle={styles.cardsGrid}>
                <View style={styles.row}>
                    {myPictures.map((pic) => (
                        <TouchableOpacity 
                            key={pic.id} 
                            disabled={hasSelected} 
                            onPress={() => handleSelectPicture(pic.id)}
                            onLongPress={() => setZoomedImage(getImageUrl(pic.picturePath) || null)}
                            delayLongPress={300}
                            style={[
                                styles.cardWrapper, 
                                selectedPictureId === pic.id && styles.selectedCard,
                                (hasSelected && selectedPictureId !== pic.id) && styles.disabledCard
                            ]}
                        >
                            <Image source={{ uri: getImageUrl(pic.picturePath) }} style={styles.cardImage} resizeMode="cover"/>
                        </TouchableOpacity>
                    ))}
                </View>
            </ScrollView>
          </>
      )}

      {/* מסך 2: הצבעה */}
      {currentState === 'voting' && (
          <>
            <Text style={styles.sectionTitle}>Vote for the funniest!</Text>
            <ScrollView contentContainerStyle={styles.cardsGrid}>
                <View style={styles.row}>
                    {roundData.picturesSelected?.map((picSelected: any) => {
                        return (
                            <TouchableOpacity 
                                key={picSelected.id}
                                disabled={hasSelected} 
                                onPress={() => handleVote(picSelected.id)}
                                onLongPress={() => setZoomedImage(getImageUrl(picSelected.picturePath) || null)}
                                delayLongPress={300}
                                style={[
                                    styles.cardWrapper,
                                    votedPictureId === picSelected.id && styles.selectedCard,
                                    (hasSelected && votedPictureId !== picSelected.id) && styles.disabledCard
                                ]}
                            >
                                <Image source={{ uri: getImageUrl(picSelected.picturePath) }} style={styles.cardImage} resizeMode="cover" />
                            </TouchableOpacity>
                        );
                    })}
                </View>
            </ScrollView>
          </>
      )}

      {/* מסך 3: תוצאות */}
      {currentState === 'ended' && (
          <ScrollView contentContainerStyle={styles.scrollContainer}>
            <Text style={styles.sectionTitle}>🏆 Round Results 🏆</Text>
            
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

            {renderLeaderboard()}
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
  fullImage: { width: '100%', height: 400, borderRadius: 10 },
  closeBtn: { marginTop: 20, backgroundColor: '#FF5252', paddingVertical: 10, paddingHorizontal: 20, borderRadius: 20 },
  closeBtnText: { color: 'white', fontWeight: 'bold', fontSize: 16 },

  trophyBtn: { position: 'absolute', top: 40, left: 20, padding: 10, backgroundColor: '#333', borderRadius: 25, zIndex: 10, borderWidth: 1, borderColor: '#555' },

  timerContainer: { position: 'absolute', top: 40, right: 20, backgroundColor: '#333', padding: 8, borderRadius: 8, alignItems: 'center', borderWidth: 1, borderColor: '#555', zIndex: 10 },
  timerLabel: { color: '#aaa', fontSize: 10, textTransform: 'uppercase' },
  timerValue: { color: 'white', fontSize: 22, fontWeight: 'bold' },
  timerUrgent: { color: '#FF5252' },

  winnerContainer: { alignItems: 'center', marginVertical: 20, width: '100%' },
  winnerImage: { width: 250, height: 250, borderRadius: 10, marginBottom: 15, borderWidth: 3, borderColor: '#FFD700' },
  winnerText: { color: '#FFD700', fontSize: 24, fontWeight: 'bold' },
  subText: { color: '#aaa', marginBottom: 20 },

  leaderboard: { width: '100%', backgroundColor: '#1E1E1E', borderRadius: 15, padding: 15, marginBottom: 20 },
  leaderboardTitle: { color: 'white', fontSize: 18, fontWeight: 'bold', marginBottom: 10, borderBottomWidth: 1, borderBottomColor: '#333', paddingBottom: 5, textAlign: 'center' },
  scoreRow: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: 8 },
  scoreName: { color: 'white', fontSize: 16 },
  scoreValue: { color: '#03DAC6', fontSize: 16, fontWeight: 'bold' },

  sentenceCard: { backgroundColor: '#1E1E1E', padding: 20, borderRadius: 20, width: '100%', minHeight: 120, justifyContent: 'center', alignItems: 'center', marginTop: 50, marginBottom: 20, borderWidth: 1, borderColor: '#333' },
  sentenceText: { color: '#fff', fontSize: 22, fontWeight: 'bold', textAlign: 'center' },
  sectionTitle: { color: '#03DAC6', fontSize: 20, marginBottom: 15, fontWeight: 'bold', alignSelf: 'flex-start' },
  cardsGrid: { paddingBottom: 50 },
  row: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'center', gap: 10 },
  cardWrapper: { borderRadius: 10, overflow: 'hidden', borderWidth: 2, borderColor: 'transparent', marginBottom: 10 },
  selectedCard: { borderColor: '#03DAC6', transform: [{ scale: 1.05 }] },
  disabledCard: { opacity: 0.5 },
  cardImage: { width: 100, height: 100, backgroundColor: '#333' }
});

export default GameScreen;