import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, Alert, ActivityIndicator, ScrollView } from 'react-native';
import axios from 'axios';

const RegisterScreen = ({ navigation }: any) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [email, setEmail] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  
  const [loading, setLoading] = useState(false);

  // הכתובת שאומתה לפי הקוד שלך בשרת
  // וודא שה-IP (10.0.0.1) והפורט (5299) עדיין נכונים אצלך
  const API_URL = "http://10.0.0.1:5299/api/Users/Register"; 

  const handleRegister = async () => {
    if (!username || !password || !email || !firstName || !lastName) {
      Alert.alert("Error", "Please fill in all fields");
      return;
    }

    setLoading(true);

    try {
      console.log("Sending registration request to:", API_URL);

      // שליחת הנתונים לשרת
      const response = await axios.post(API_URL, {
        username: username,
        password: password,
        email: email,
        firstName: firstName,
        lastName: lastName
      });

      console.log("Registration Response:", response.data);

      if (response.status === 201 || response.status === 200) {
        Alert.alert("Success", "Account created successfully! Please login.", [
            { text: "OK", onPress: () => navigation.navigate("Login") }
        ]);
      }
    } catch (error: any) {
      console.error("Registration Error:", error);
      
      // ניסיון לחלץ הודעת שגיאה ברורה מהשרת
      let errorMsg = "Registration failed";
      if (error.response) {
          if (error.response.data && error.response.data.message) {
              errorMsg = error.response.data.message; // הודעה מה-APIResponse
          } else if (typeof error.response.data === 'string') {
              errorMsg = error.response.data;
          }
      } else if (error.message) {
          errorMsg = error.message;
      }

      Alert.alert("Error", errorMsg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <ScrollView contentContainerStyle={styles.scrollContent}>
        <Text style={styles.title}>Create Account</Text>

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
            placeholder="Email"
            placeholderTextColor="#aaa"
            value={email}
            onChangeText={setEmail}
            keyboardType="email-address"
            autoCapitalize="none"
        />

        <TextInput
            style={styles.input}
            placeholder="First Name"
            placeholderTextColor="#aaa"
            value={firstName}
            onChangeText={setFirstName}
        />

        <TextInput
            style={styles.input}
            placeholder="Last Name"
            placeholderTextColor="#aaa"
            value={lastName}
            onChangeText={setLastName}
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
            onPress={handleRegister}
            disabled={loading}
        >
            {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Sign Up</Text>}
        </TouchableOpacity>

        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.linkButton}>
            <Text style={styles.linkText}>Already have an account? Login</Text>
        </TouchableOpacity>
      </ScrollView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#121212',
  },
  scrollContent: {
    padding: 20,
    justifyContent: 'center',
    minHeight: '100%',
  },
  title: {
    fontSize: 32,
    color: '#fff',
    fontWeight: 'bold',
    marginBottom: 30,
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
    backgroundColor: '#03DAC6',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 10,
  },
  buttonText: {
    color: '#000',
    fontSize: 18,
    fontWeight: 'bold',
  },
  linkButton: {
    marginTop: 20,
    alignItems: 'center',
  },
  linkText: {
    color: '#BB86FC',
    fontSize: 16,
  }
});

export default RegisterScreen;