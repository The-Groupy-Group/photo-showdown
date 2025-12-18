import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, Alert, ActivityIndicator } from 'react-native';
import usersService from '../services/usersService';
import * as SecureStore from 'expo-secure-store';
import { jwtDecode } from "jwt-decode";

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
      console.log("Attempting login...");
      
      // שימוש ב-Service
      const response = await usersService.login({
        username: username,
        password: password
      });

      console.log("Login Success. Response data:", response.data);

      if (response.data.isSuccess && response.data.data) {
        const token = response.data.data.token;
        const decoded = jwtDecode<TokenPayload>(token);
        const userId = parseInt(decoded.Id);
        const decodedUsername = decoded.Username;

        await SecureStore.setItemAsync('userToken', token);
        await SecureStore.setItemAsync('userId', userId.toString());

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
      const msg = error.response?.data?.message || error.message || "Connection failed. Check IP/Server.";
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
    color: '#03DAC6',
    fontWeight: 'bold',
  }
});

export default LoginScreen;