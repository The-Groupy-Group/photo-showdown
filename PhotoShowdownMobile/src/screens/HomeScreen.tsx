import React, { useEffect, useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, TextInput, Alert, ActivityIndicator } from 'react-native';
import axios from 'axios';
import * as SecureStore from 'expo-secure-store';

const HomeScreen = ({ route, navigation }: any) => {
  const { token, userId, username } = route.params;

  const [matchIdInput, setMatchIdInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [checkingStatus, setCheckingStatus] = useState(true);

  const API_BASE = "http://10.0.0.1:5299/api/Matches";
  const authHeader = { headers: { Authorization: `Bearer ${token}` } };

  useEffect(() => {
    checkStatusAndCleanup();
  }, []);

  // --- לוגיקה חדשה: חיפוש וניקוי חכם ---
  // --- החלף את הפונקציה הקיימת בזו ---
  const checkStatusAndCleanup = async () => {
    try {
      // בדיקה רגילה
      const response = await axios.get(`${API_BASE}/GetCurrentMatch`, authHeader);
      if (response.data.data) {
           promptRejoinOrLeave(response.data.data.id);
      }
    } catch (e: any) {
      // אם קיבלנו 500, סימן שאנחנו תקועים במשחק שבור
      if (e.response?.status === 500) {
          console.log("Detected broken match (500). Starting BRUTE FORCE cleanup...");
          await bruteForceLeave();
      }
    } finally {
      setCheckingStatus(false);
    }
  };

  // --- פונקציה חדשה: מנסה לצאת מכל משחק אפשרי ---
  const bruteForceLeave = async () => {
      setLoading(true);
      // ראינו שהגעת למשחק 28, אז טווח של 1-50 יכסה הכל
      const promises = [];
      for (let i = 1; i <= 50; i++) {
          // שולחים את הבקשות במקביל כדי שזה יהיה מהיר
          promises.push(
              axios.delete(`${API_BASE}/LeaveMatch/${i}`, authHeader)
                .then(() => console.log(`Deleted match connection: ${i}`))
                .catch(() => {}) // מתעלמים משגיאות (כי ברוב המשחקים אנחנו לא נמצאים)
          );
      }
      
      await Promise.all(promises);
      Alert.alert("Cleanup Done", "Force cleaned all connections. Try creating a match now.");
      setLoading(false);
  };

  // --- הפתרון העוקף: חיפוש ידני בכל המשחקים ---
  const findAndLeaveBrokenMatch = async () => {
      try {
          // מבקשים את כל המשחקים הפתוחים
          const res = await axios.get(`${API_BASE}/GetAllMatches`, authHeader);
          const allMatches = res.data.data || [];

          // מחפשים משחק שהמשתמש שלנו נמצא בתוכו
          // (מניחים ש-MatchDTO מכיל רשימת users)
          const myMatch = allMatches.find((m: any) => 
              m.users && m.users.some((u: any) => u.id === userId)
          );

          if (myMatch) {
              console.log(`Found user in match ${myMatch.id} via list scan.`);
              // יציאה בכוח
              await forceLeaveMatch(myMatch.id);
          } else {
              console.log("Could not find user in any match list.");
          }

      } catch (err) {
          console.error("Failed to scan matches:", err);
      }
  };

  const promptRejoinOrLeave = (matchId: number) => {
    Alert.alert(
        "Active Game Found",
        `You are in Match ${matchId}.`,
        [
            { text: "Leave Match", style: "destructive", onPress: () => forceLeaveMatch(matchId) },
            { text: "Rejoin", onPress: () => goToLobby(matchId) }
        ]
    );
  };

  const forceLeaveMatch = async (matchId: number) => {
      setLoading(true);
      try {
          console.log(`Force leaving match ${matchId}...`);
          await axios.delete(`${API_BASE}/LeaveMatch/${matchId}`, authHeader);
          Alert.alert("Fixed!", "Cleaned up stuck match. Try creating a new one.");
      } catch (e) {
          console.error("Failed to leave match:", e);
          Alert.alert("Error", "Could not clear match. Please restart app.");
      } finally {
          setLoading(false);
      }
  };

  const handleCreateMatch = async () => {
    setLoading(true);
    try {
      console.log("Creating new match (Lobby Mode)...");
      
      // 1. יצירת המשחק בלבד
      const response = await axios.post(`${API_BASE}/CreateNewMatch`, {}, authHeader);
      
      if (response.data.isSuccess) {
        const newMatch = response.data.data;
        console.log("Match Created:", newMatch.id);
        
        // 2. מעבר ללובי (בלי להתחיל את המשחק!)
        // בלובי נחכה ששחקנים אחרים יצטרפו
        goToLobby(newMatch.id);
      }
    } catch (error: any) {
       console.log("Create failed. Trying auto-fix...");
       await findAndLeaveBrokenMatch();
       Alert.alert("Notice", "Cleaned up old session. Please try 'Create Match' again.");
    } finally {
      setLoading(false);
    }
  };

  const handleJoinMatch = async () => {
    if (!matchIdInput) {
      Alert.alert("Error", "Please enter a Match ID");
      return;
    }

    setLoading(true);
    try {
      const url = `${API_BASE}/JoinMatch/${matchIdInput}`;
      await axios.post(url, {}, authHeader);
      goToLobby(parseInt(matchIdInput));
    } catch (error: any) {
      Alert.alert("Error", "Could not join match.");
    } finally {
      setLoading(false);
    }
  };

  const goToLobby = (matchId: number) => {
    navigation.replace('Lobby', {
      matchId: matchId,
      token: token,
      userId: userId,
      username: username
    });
  };

  const handleLogout = async () => {
      // מנסים לנקות לפני יציאה
      await findAndLeaveBrokenMatch();
      await SecureStore.deleteItemAsync('userToken');
      navigation.replace('Login');
  };

  if (checkingStatus) {
    return (
      <View style={[styles.container, { justifyContent: 'center' }]}>
        <ActivityIndicator size="large" color="#6200EE" />
        <Text style={{color:'white', marginTop:10}}>Cleaning up session...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Text style={styles.header}>Hello, {username} 👋</Text>
      
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Start a New Game</Text>
        <Text style={styles.cardDesc}>Be the host and invite friends.</Text>
        <TouchableOpacity 
          style={[styles.button, styles.createButton]} 
          onPress={handleCreateMatch}
          disabled={loading}
        >
           {loading ? <ActivityIndicator color="#fff"/> : <Text style={styles.btnText}>Create Match</Text>}
        </TouchableOpacity>
      </View>

      <Text style={styles.orText}>- OR -</Text>

      <View style={styles.card}>
        <Text style={styles.cardTitle}>Join a Game</Text>
        <TextInput
          style={styles.input}
          placeholder="Enter Match ID"
          placeholderTextColor="#888"
          keyboardType="numeric"
          value={matchIdInput}
          onChangeText={setMatchIdInput}
        />
        <TouchableOpacity 
          style={[styles.button, styles.joinButton]} 
          onPress={handleJoinMatch}
          disabled={loading}
        >
          {loading ? <ActivityIndicator color="#fff"/> : <Text style={styles.btnText}>Join Match</Text>}
        </TouchableOpacity>
      </View>

      <TouchableOpacity onPress={handleLogout} style={styles.logoutBtn}>
        <Text style={styles.logoutText}>Logout</Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#121212',
    padding: 20,
    alignItems: 'center',
    paddingTop: 60,
  },
  header: {
    fontSize: 28,
    color: 'white',
    fontWeight: 'bold',
    marginBottom: 40,
  },
  card: {
    width: '100%',
    backgroundColor: '#1E1E1E',
    padding: 20,
    borderRadius: 15,
    alignItems: 'center',
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 5,
  },
  cardTitle: {
    color: 'white',
    fontSize: 20,
    fontWeight: 'bold',
    marginBottom: 5,
  },
  cardDesc: {
    color: '#aaa',
    fontSize: 14,
    marginBottom: 20,
  },
  input: {
    width: '100%',
    backgroundColor: '#333',
    color: 'white',
    padding: 15,
    borderRadius: 8,
    marginBottom: 15,
    textAlign: 'center',
    fontSize: 18,
  },
  button: {
    width: '100%',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
  },
  createButton: {
    backgroundColor: '#6200EE',
  },
  joinButton: {
    backgroundColor: '#03DAC6',
  },
  btnText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: 16,
  },
  orText: {
    color: '#555',
    marginVertical: 20,
    fontWeight: 'bold',
  },
  logoutBtn: {
    marginTop: 40,
  },
  logoutText: {
    color: '#FF5252',
    fontSize: 16,
  },
});

export default HomeScreen;