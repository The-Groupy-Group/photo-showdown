import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, Alert, ActivityIndicator } from 'react-native';
import axios from 'axios';
import * as SecureStore from 'expo-secure-store';
import { jwtDecode } from "jwt-decode";
import { API_BASE } from '../utils/Config';

interface TokenPayload {
  Id: string;
  Username: string;
  exp: number;
}

const LoginScreen = ({ navigation }: any) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  

  const handleLogin = async () => {
    if (!username || !password) {
      Alert.alert("Error", "Please fill in all fields");
      return;
    }

    setLoading(true);

    try {
      console.log("Attempting login to:", API_BASE);
      
      const response = await axios.post(API_BASE, {
        username: username,
        password: password
      });

      console.log("Login Success. Response data:", response.data);

      if (response.data.isSuccess && response.data.data) {
        const token = response.data.data.token;

        // פענוח הטוקן כדי להשיג את ה-ID והשם
        const decoded = jwtDecode<TokenPayload>(token);
        console.log("Decoded Token:", decoded);

        const userId = parseInt(decoded.Id);
        const decodedUsername = decoded.Username;

        // שמירה בטוחה בזיכרון המכשיר
        await SecureStore.setItemAsync('userToken', token);
        await SecureStore.setItemAsync('userId', userId.toString());

        // --- השינוי החשוב: ניווט למסך העלאת התמונות ---
        // אנחנו חייבים לעבור שם קודם כדי למנוע קריסה במשחק
        navigation.replace('ManagePicturesScreen', {
          token: token,
          username: decodedUsername,
          userId: userId
        });

      } else {
        Alert.alert("Login Failed", response.data.message || "Unknown error");
      }

    } catch (error: any) {
      console.error("Login Error:", error);
      const msg = error.response?.data?.message || "Connection failed. Check IP/Server.";
      Alert.alert("Login Failed", msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Photo Showdown 📸</Text>
      
      <TextInput
        style={styles.input}
        placeholder="Username"
        placeholderTextColor="#aaa"
        value={username}
        onChangeText={setUsername}
        autoCapitalize="none"
      />
      
      <TextInput
        style={styles.input}
        placeholder="Password"
        placeholderTextColor="#aaa"
        value={password}
        onChangeText={setPassword}
        secureTextEntry
      />

      <TouchableOpacity 
        style={styles.button} 
        onPress={handleLogin}
        disabled={loading}
      >
        {loading ? (
          <ActivityIndicator color="#fff" />
        ) : (
          <Text style={styles.buttonText}>Login</Text>
        )}
      </TouchableOpacity>

      {/* כפתור למעבר למסך ההרשמה */}
      <TouchableOpacity 
        style={styles.registerLink} 
        onPress={() => navigation.navigate('Register')}
      >
        <Text style={styles.registerText}>
          Don't have an account? <Text style={styles.registerHighlight}>Sign Up</Text>
        </Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#121212',
    justifyContent: 'center',
    padding: 20,
  },
  title: {
    fontSize: 32,
    color: '#fff',
    fontWeight: 'bold',
    marginBottom: 40,
    textAlign: 'center',
  },
  input: {
    backgroundColor: '#333',
    color: '#fff',
    padding: 15,
    borderRadius: 10,
    marginBottom: 15,
    fontSize: 16,
  },
  button: {
    backgroundColor: '#6200EE',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 10,
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
  registerLink: {
    marginTop: 25,
    alignItems: 'center',
  },
  registerText: {
    color: '#aaa',
    fontSize: 16,
  },
  registerHighlight: {
    color: '#03DAC6', // צבע טורקיז בולט
    fontWeight: 'bold',
  }
});

export default LoginScreen;