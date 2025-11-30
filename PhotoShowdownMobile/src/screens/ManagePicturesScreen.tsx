import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, FlatList, Image, Alert, ActivityIndicator } from 'react-native';
import * as ImagePicker from 'expo-image-picker';
import axios from 'axios';

const ManagePicturesScreen = ({ navigation, route }: any) => {
  const { token, userId, username } = route.params;
  const [myPictures, setMyPictures] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const API_BASE = "http://10.0.0.1:5299/api/Pictures";
  const authHeader = { 
      headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'multipart/form-data' 
      } 
  };

  useEffect(() => {
    fetchMyPictures();
  }, []);

  const fetchMyPictures = async () => {
    try {
      // קריאה לקבלת התמונות הקיימות
      const response = await axios.get(`${API_BASE}/GetMyPictures`, {
          headers: { 'Authorization': `Bearer ${token}` }
      });
      if (response.data.data) {
          setMyPictures(response.data.data);
      }
    } catch (error) {
      console.log("Error fetching pictures:", error);
    }
  };

  const pickImage = async () => {
    // בקשת הרשאה
    const permissionResult = await ImagePicker.requestMediaLibraryPermissionsAsync();
    if (permissionResult.granted === false) {
      Alert.alert("Permission to access camera roll is required!");
      return;
    }

    const result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsMultipleSelection: true, // מאפשר לבחור כמה ביחד (אם נתמך)
      quality: 0.5, // כיווץ כדי לא להכביד על ההעלאה
    });

    if (!result.canceled) {
      uploadImages(result.assets);
    }
  };

  const uploadImages = async (assets: ImagePicker.ImagePickerAsset[]) => {
    setLoading(true);
    const formData = new FormData();

    assets.forEach((asset, index) => {
        // חייבים להמיר את ה-URI לאובייקט ש-FormData מבין
        const file = {
            uri: asset.uri,
            name: `photo_${index}.jpg`,
            type: 'image/jpeg'
        } as any;
        
        formData.append('pictureFiles', file);
    });

    try {
      console.log("Uploading images...");
      await axios.post(`${API_BASE}/UploadPicture`, formData, authHeader);
      Alert.alert("Success", "Pictures uploaded!");
      fetchMyPictures(); // רענון הרשימה
    } catch (error: any) {
      console.error("Upload failed:", error);
      Alert.alert("Error", "Failed to upload pictures.");
    } finally {
      setLoading(false);
    }
  };

  const handleContinue = () => {
      if (myPictures.length < 5) {
          Alert.alert("Wait!", "You need at least 5 pictures to play properly.");
          return;
      }
      // מעבר למסך הבית האמיתי
      navigation.replace('Home', { token, userId, username });
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>My Collection 🖼️</Text>
      <Text style={styles.subtitle}>Upload funny pictures to play with!</Text>

      <FlatList
        data={myPictures}
        keyExtractor={(item) => item.id.toString()}
        numColumns={3}
        contentContainerStyle={styles.grid}
        renderItem={({ item }) => (
          <Image 
            source={{ uri: `http://10.0.0.1:5299/${item.picturePath}` }} 
            style={styles.imageThumbnail} 
          />
        )}
      />

      <View style={styles.footer}>
          <TouchableOpacity style={styles.uploadBtn} onPress={pickImage} disabled={loading}>
            {loading ? <ActivityIndicator color="#fff"/> : <Text style={styles.btnText}>+ Upload Pictures</Text>}
          </TouchableOpacity>

          <TouchableOpacity style={styles.continueBtn} onPress={handleContinue}>
            <Text style={styles.btnText}>Continue to Game 👉</Text>
          </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#121212',
    padding: 20,
    paddingTop: 50
  },
  title: {
    fontSize: 28,
    color: 'white',
    fontWeight: 'bold',
    textAlign: 'center'
  },
  subtitle: {
    fontSize: 14,
    color: '#aaa',
    textAlign: 'center',
    marginBottom: 20
  },
  grid: {
    paddingBottom: 100
  },
  imageThumbnail: {
    width: 100,
    height: 100,
    margin: 5,
    borderRadius: 8,
    backgroundColor: '#333'
  },
  footer: {
    position: 'absolute',
    bottom: 20,
    left: 20,
    right: 20,
    gap: 10
  },
  uploadBtn: {
    backgroundColor: '#6200EE',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center'
  },
  continueBtn: {
    backgroundColor: '#03DAC6',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center'
  },
  btnText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: 16
  }
});

export default ManagePicturesScreen;