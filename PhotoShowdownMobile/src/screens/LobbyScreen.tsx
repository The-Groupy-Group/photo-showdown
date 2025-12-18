import React, { useEffect, useState, useRef } from 'react';
import { 
  View, Text, StyleSheet, FlatList, TouchableOpacity, Alert, 
  ActivityIndicator, Modal, TextInput, ScrollView, KeyboardAvoidingView, Platform, BackHandler 
} from 'react-native';
import matchesService from '../services/matchesService';
import { GameState } from '../enums/GameState';

interface IPlayer {
  id: number;
  username: string;
  isHost: boolean;
}

const LobbyScreen = ({ route, navigation }: any) => {
  const { matchId, token, userId, username } = route.params;
  const [players, setPlayers] = useState<IPlayer[]>([]);
  const [loading, setLoading] = useState(false);
  
  const [showSettings, setShowSettings] = useState(false);
  const [numRounds, setNumRounds] = useState('5');
  const [votesToWin, setVotesToWin] = useState('3');
  
  const [selectionTime, setSelectionTime] = useState('60'); 
  const [voteTime, setVoteTime] = useState('60');       
  
  const [customSentences, setCustomSentences] = useState<string[]>([]);
  const [newSentence, setNewSentence] = useState('');

  const intervalRef = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    const backAction = () => {
      handleLeaveMatch();
      return true;
    };
    const backHandler = BackHandler.addEventListener('hardwareBackPress', backAction);
    return () => backHandler.remove();
  }, []);

  useEffect(() => {
    fetchLobbyStatus();
    intervalRef.current = setInterval(fetchLobbyStatus, 3000);
    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, []);

 const fetchLobbyStatus = async () => {
    try {
      const response = await matchesService.getMatchById(matchId, token);
      
      if (response.data.data) {
        const matchData = response.data.data;
        

        const rawState = matchData.MatchState !== undefined ? matchData.MatchState : matchData.matchState;
        
        console.log(`📡 Status Check: Value=${rawState}, Type=${typeof rawState}`);

        let isGameStarted = false;

        if (rawState === 1) {
            isGameStarted = true;
        }
        else if (typeof rawState === 'string' && rawState.toLowerCase() === 'inprogress') {
            isGameStarted = true;
        }

        if (isGameStarted) { 
             console.log("🚀 GAME STARTED DETECTED! Navigating...");
             goToGameScreen();
             return;
        }


        const ownerObject = matchData.Owner || matchData.owner; 
        const ownerId = ownerObject ? (ownerObject.Id || ownerObject.id) : -1;

        const mappedPlayers = (matchData.Users || matchData.users).map((u: any) => ({
            id: u.Id || u.id,
            username: u.Username || u.username,
            isHost: (u.Id || u.id) == ownerId 
        }));
        
        setPlayers(mappedPlayers);
      }
    } catch (error) {
      console.log("Polling error (ignoring):", error);
    }
  };

  const goToGameScreen = () => {
    if (intervalRef.current) clearInterval(intervalRef.current);
    navigation.replace('GameScreen', { matchId, token, userId });
  };

  const goToManagePictures = () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
      navigation.navigate('ManagePicturesScreen', {
          token, userId, username,
          fromScreen: 'Lobby', matchId
      });
  };

  const addSentence = () => {
      if (newSentence.trim().length > 0) {
          setCustomSentences([...customSentences, newSentence]);
          setNewSentence('');
      }
  };

  const removeSentence = (index: number) => {
      const updated = [...customSentences];
      updated.splice(index, 1);
      setCustomSentences(updated);
  };

 const handleStartGame = async () => {
    setLoading(true);
    try {
      console.log("Host starting game...");
      
      const roundsInt = parseInt(numRounds) || 5;
      const votesInt = parseInt(votesToWin) || 3;
      const selectionTimeInt = parseInt(selectionTime) || 60;
      const voteTimeInt = parseInt(voteTime) || 60;
      const finalSentences = customSentences; 

      const gameConfig = {
          MatchId: matchId,
          Sentences: finalSentences,
          NumOfRounds: roundsInt,
          NumOfVotesToWin: votesInt,
          PictureSelectionTimeSeconds: selectionTimeInt,
          VoteTimeSeconds: voteTimeInt,
          matchId: matchId,
          sentences: finalSentences,
          numOfRounds: roundsInt,
          numOfVotesToWin: votesInt,
          pictureSelectionTimeSeconds: selectionTimeInt,
          voteTimeSeconds: voteTimeInt
      };


      await matchesService.startMatch(gameConfig, token);
      
      console.log("Start command sent successfully. Waiting for polling to pick it up...");
      Alert.alert("Success", "Game is starting...");
      
      setTimeout(fetchLobbyStatus, 500); 

    } catch (error: any) {
      console.error("Start Game Error:", error.response?.data || error.message);
      let errorMsg = "Failed to start game.";
      if (error.response?.data?.message) {
          errorMsg += "\nServer: " + error.response.data.message;
      }
      Alert.alert("Error", errorMsg);
      setLoading(false);
    }
  };

  const handleLeaveMatch = () => {
    Alert.alert(
      "Leave Lobby",
      "Are you sure?",
      [
        { text: "Cancel", style: "cancel" },
        { 
          text: "Leave", 
          style: "destructive", 
          onPress: async () => {
            if (intervalRef.current) clearInterval(intervalRef.current);
            try {
                await matchesService.leaveMatch(matchId, token);
            } catch (e) {} finally {
                navigation.replace('Home', { token, userId, username });
            }
          }
        }
      ]
    );
  };

  const amIHost = players.find(p => p.id === userId)?.isHost;

  const renderPlayerItem = ({ item }: { item: IPlayer }) => (
    <View style={styles.playerContainer}>
      <View style={[styles.avatarCircle, item.isHost && styles.hostBorder]}>
        <Text style={styles.avatarText}>
          {item.username.charAt(0).toUpperCase()}
        </Text>
      </View>
      <Text style={styles.playerName} numberOfLines={1}>{item.username}</Text>
      {item.isHost && <Text style={styles.hostLabel}>HOST</Text>}
    </View>
  );

  return (
    <View style={styles.container}>
      
      <Modal visible={showSettings} animationType="slide" transparent={true}>
          <KeyboardAvoidingView behavior={Platform.OS === "ios" ? "padding" : "height"} style={styles.modalOverlay}>
              <View style={styles.modalContent}>
                  <Text style={styles.modalTitle}>Game Settings ⚙️</Text>
                  <ScrollView style={{width: '100%'}}>
                      <Text style={styles.label}>Number of Rounds:</Text>
                      <TextInput style={styles.input} value={numRounds} onChangeText={setNumRounds} keyboardType="numeric" />
                      <Text style={styles.label}>Votes to Win:</Text>
                      <TextInput style={styles.input} value={votesToWin} onChangeText={setVotesToWin} keyboardType="numeric" />
                      <Text style={styles.label}>Time to Pick Picture (sec):</Text>
                      <TextInput style={styles.input} value={selectionTime} onChangeText={setSelectionTime} keyboardType="numeric" />
                      <Text style={styles.label}>Time to Vote (sec):</Text>
                      <TextInput style={styles.input} value={voteTime} onChangeText={setVoteTime} keyboardType="numeric" />
                      <Text style={[styles.label, {marginTop: 20}]}>Custom Sentences (Optional):</Text>
                      <View style={styles.addSentenceContainer}>
                          <TextInput style={[styles.input, {flex:1, marginBottom:0}]} placeholder="Type a funny sentence..." placeholderTextColor="#666" value={newSentence} onChangeText={setNewSentence}/>
                          <TouchableOpacity style={styles.addBtn} onPress={addSentence}><Text style={styles.addBtnText}>+</Text></TouchableOpacity>
                      </View>
                      {customSentences.map((s, index) => (
                          <View key={index} style={styles.sentenceRow}>
                              <Text style={styles.sentenceText} numberOfLines={1}>{s}</Text>
                              <TouchableOpacity onPress={() => removeSentence(index)}><Text style={styles.removeText}>✕</Text></TouchableOpacity>
                          </View>
                      ))}
                  </ScrollView>
                  <TouchableOpacity style={styles.closeSettingsBtn} onPress={() => setShowSettings(false)}>
                      <Text style={styles.closeBtnText}>Save & Close</Text>
                  </TouchableOpacity>
              </View>
          </KeyboardAvoidingView>
      </Modal>

      <View style={styles.header}>
          <Text style={styles.title}>Lobby</Text>
          <Text style={styles.subTitle}>Match ID: {matchId}</Text>
      </View>
      
      <Text style={styles.waitingText}>Updating automatically...</Text>

      <FlatList
        data={players}
        renderItem={renderPlayerItem}
        keyExtractor={(item) => item.id.toString()}
        numColumns={3}
        contentContainerStyle={styles.gridContainer}
        columnWrapperStyle={styles.row}
      />

      <View style={styles.footer}>
        {amIHost ? (
            <>
                <TouchableOpacity style={styles.settingsButton} onPress={() => setShowSettings(true)}>
                    <Text style={styles.settingsButtonText}>⚙️ Game Settings</Text>
                </TouchableOpacity>
                <TouchableOpacity style={styles.startButton} onPress={handleStartGame} disabled={loading}>
                    {loading ? <ActivityIndicator color="white"/> : <Text style={styles.startButtonText}>Start Game</Text>}
                </TouchableOpacity>
            </>
        ) : (
            <Text style={styles.infoText}>Waiting for host to start...</Text>
        )}
        <TouchableOpacity style={styles.galleryButton} onPress={goToManagePictures}>
             <Text style={styles.galleryButtonText}>🖼️ Manage Pictures</Text>
        </TouchableOpacity>
        <TouchableOpacity style={styles.leaveButton} onPress={handleLeaveMatch}>
            <Text style={styles.leaveButtonText}>Leave Lobby</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#121212', padding: 20, alignItems: 'center' },
  header: { marginTop: 40, alignItems: 'center' },
  title: { fontSize: 32, fontWeight: 'bold', color: '#ffffff' },
  subTitle: { fontSize: 18, color: '#aaaaaa', marginTop: 5 },
  waitingText: { fontSize: 12, color: '#4CAF50', marginVertical: 20 },
  gridContainer: { width: '100%', flexGrow: 0 },
  row: { justifyContent: 'space-around', marginBottom: 20 },
  playerContainer: { alignItems: 'center', width: 90 },
  avatarCircle: { width: 60, height: 60, borderRadius: 30, backgroundColor: '#333', justifyContent: 'center', alignItems: 'center', borderWidth: 2, borderColor: '#555', marginBottom: 8 },
  hostBorder: { borderColor: '#FFD700' },
  avatarText: { fontSize: 24, color: 'white', fontWeight: 'bold' },
  playerName: { color: 'white', fontSize: 14, textAlign: 'center' },
  hostLabel: { color: '#FFD700', fontSize: 10, marginTop: 2, fontWeight: 'bold' },
  footer: { width: '100%', marginTop: 'auto', marginBottom: 20, alignItems: 'center' },
  startButton: { backgroundColor: '#6200EE', paddingVertical: 15, width: '100%', borderRadius: 25, alignItems: 'center', marginBottom: 15 },
  startButtonText: { color: 'white', fontSize: 18, fontWeight: 'bold' },
  settingsButton: { backgroundColor: '#333', paddingVertical: 12, width: '100%', borderRadius: 25, alignItems: 'center', marginBottom: 15, borderWidth: 1, borderColor: '#555' },
  settingsButtonText: { color: '#03DAC6', fontSize: 16, fontWeight: '600' },
  galleryButton: { backgroundColor: '#444', paddingVertical: 12, width: '100%', borderRadius: 25, alignItems: 'center', marginBottom: 15, borderWidth: 1, borderColor: '#666' },
  galleryButtonText: { color: '#fff', fontSize: 16, fontWeight: '600' },
  leaveButton: { paddingVertical: 10, width: '100%', alignItems: 'center' },
  leaveButtonText: { color: '#FF5252', fontSize: 16, fontWeight: '600' },
  infoText: { color: '#888', marginBottom: 20, fontSize: 16 },
  modalOverlay: { flex: 1, backgroundColor: 'rgba(0,0,0,0.8)', justifyContent: 'center', alignItems: 'center', padding: 20 },
  modalContent: { width: '100%', backgroundColor: '#1E1E1E', borderRadius: 20, padding: 20, alignItems: 'center', maxHeight: '90%' },
  modalTitle: { color: 'white', fontSize: 24, fontWeight: 'bold', marginBottom: 20 },
  label: { color: '#aaa', alignSelf: 'flex-start', marginBottom: 5, marginTop: 10 },
  input: { backgroundColor: '#333', width: '100%', color: 'white', padding: 12, borderRadius: 10, marginBottom: 5 },
  closeSettingsBtn: { marginTop: 20, backgroundColor: '#03DAC6', paddingVertical: 12, paddingHorizontal: 30, borderRadius: 20, width: '100%', alignItems: 'center' },
  closeBtnText: { color: 'black', fontWeight: 'bold', fontSize: 16 },
  addSentenceContainer: { flexDirection: 'row', alignItems: 'center', gap: 10, marginBottom: 10 },
  addBtn: { backgroundColor: '#6200EE', width: 50, height: 50, borderRadius: 25, justifyContent: 'center', alignItems: 'center' },
  addBtnText: { color: 'white', fontSize: 24, fontWeight: 'bold' },
  sentenceRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', backgroundColor: '#2A2A2A', padding: 10, borderRadius: 8, marginBottom: 5 },
  sentenceText: { color: 'white', flex: 1, marginRight: 10 },
  removeText: { color: '#FF5252', fontSize: 18, fontWeight: 'bold' }
});

export default LobbyScreen;