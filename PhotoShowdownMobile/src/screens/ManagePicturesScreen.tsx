import React, { useState, useEffect } from 'react';
import { 
  View, Text, StyleSheet, TouchableOpacity, FlatList, Image, 
  Alert, ActivityIndicator, Modal, SafeAreaView 
} from 'react-native';
import * as ImagePicker from 'expo-image-picker';
import api from '../services/api';
import { IP_ADDRESS, PORT } from '../config'; 

const ManagePicturesScreen = ({ navigation, route }: any) => {
  const { token, userId, username } = route.params;
  const [myPictures, setMyPictures] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  // משתנה לניהול התמונה שנבחרה להגדלה
  const [selectedImage, setSelectedImage] = useState<any>(null);

  const authHeader = { 
      headers: { 
          'Authorization': `Bearer ${token}`,
          // ב-Delete וב-Get לא צריך content-type, אבל זה לא מזיק
      } 
  };

  const uploadHeader = {
      headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'multipart/form-data' 
      } 
  };

  useEffect(() => {
    fetchMyPictures();
  }, [refreshKey]); 

  const fetchMyPictures = async () => {
    try {
      const response = await api.get(`/Pictures/GetMyPictures`, authHeader);
      if (response.data.data) {
          setMyPictures(response.data.data);
      }
    } catch (error) {
      console.log("Error fetching pictures:", error);
    }
  };

  const getImageUrl = (path: string) => {
      if (!path) return null;
      let cleanPath = path.replace(/\\/g, '/');
      if (cleanPath.startsWith('/')) {
          cleanPath = cleanPath.substring(1);
      }
      if (!cleanPath.startsWith('pictures/')) {
          cleanPath = `pictures/${cleanPath}`;
      }
      return `http://${IP_ADDRESS}:${PORT}/${cleanPath}`;
  };

  const pickImage = async () => {
    const permissionResult = await ImagePicker.requestMediaLibraryPermissionsAsync();
    if (permissionResult.granted === false) {
      Alert.alert("Permission to access camera roll is required!");
      return;
    }

    const result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      allowsMultipleSelection: true, 
      quality: 0.5, 
    });

    if (!result.canceled) {
      uploadImages(result.assets);
    }
  };

  const uploadImages = async (assets: ImagePicker.ImagePickerAsset[]) => {
    setLoading(true);
    const formData = new FormData();

    assets.forEach((asset, index) => {
        const file = {
            uri: asset.uri,
            name: `photo_${Date.now()}_${index}.jpg`,
            type: 'image/jpeg'
        } as any;
        formData.append('pictureFiles', file);
    });

    try {
      await api.post(`/Pictures/UploadPicture`, formData, uploadHeader);
      Alert.alert("Success", "Pictures uploaded!");
      setRefreshKey(prev => prev + 1); 
    } catch (error: any) {
      Alert.alert("Error", "Failed to upload pictures.");
    } finally {
      setLoading(false);
    }
  };

  // --- פונקציית המחיקה האמיתית ---
  const handleDeletePicture = async () => {
      if (!selectedImage) return;

      Alert.alert(
          "Delete Picture",
          "Are you sure you want to delete this picture?",
          [
              { text: "Cancel", style: "cancel" },
              { 
                  text: "Delete", 
                  style: "destructive", 
                  onPress: async () => {
                      try {
                          console.log("Deleting image ID:", selectedImage.id);
                          // קריאה לשרת לפי ה-Controller ששלחת
                          await api.delete(`/Pictures/DeletePicture/${selectedImage.id}`, authHeader);
                          
                          Alert.alert("Deleted", "Picture removed successfully.");
                          setSelectedImage(null); // סגירת המודל
                          setRefreshKey(prev => prev + 1); // רענון הרשימה
                      } catch (error) {
                          console.error("Delete error:", error);
                          Alert.alert("Error", "Could not delete picture.");
                      }
                  } 
              }
          ]
      );
  };

  const handleContinue = () => {
      if (myPictures.length < 5) {
          Alert.alert("Wait!", "You need at least 5 pictures to play properly.");
          return;
      }
      navigation.replace('Home', { token, userId, username });
  };

  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.container}>
        
        {/* --- MODAL להגדלת תמונה --- */}
        <Modal visible={!!selectedImage} transparent={true} animationType="fade">
            <View style={styles.modalBackground}>
                <View style={styles.modalContent}>
                    {selectedImage && (
                        <Image 
                            source={{ uri: getImageUrl(selectedImage.picturePath) || undefined }} 
                            style={styles.fullImage} 
                            resizeMode="contain"
                        />
                    )}
                    
                    <View style={styles.modalButtons}>
                        <TouchableOpacity style={styles.deleteBtn} onPress={handleDeletePicture}>
                            <Text style={styles.btnText}>🗑️ Delete</Text>
                        </TouchableOpacity>

                        <TouchableOpacity style={styles.closeBtn} onPress={() => setSelectedImage(null)}>
                            <Text style={styles.btnText}>Close</Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </View>
        </Modal>

        <Text style={styles.title}>My Collection 🖼️</Text>
        <Text style={styles.subtitle}>Upload funny pictures to play with!</Text>

        <FlatList
          data={myPictures}
          keyExtractor={(item) => item.id.toString()}
          numColumns={3}
          contentContainerStyle={styles.grid}
          renderItem={({ item }) => {
              const imgUri = getImageUrl(item.picturePath);
              return (
                <TouchableOpacity onPress={() => setSelectedImage(item)}>
                    <View style={styles.imageContainer}>
                        <Image 
                          source={{ uri: imgUri || undefined }} 
                          style={styles.imageThumbnail} 
                          resizeMode="cover"
                        />
                    </View>
                </TouchableOpacity>
              );
          }}
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
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: '#121212',
  },
  container: {
    flex: 1,
    padding: 20,
  },
  title: {
    fontSize: 28,
    color: 'white',
    fontWeight: 'bold',
    textAlign: 'center',
    marginTop: 20,
  },
  subtitle: {
    fontSize: 14,
    color: '#aaa',
    textAlign: 'center',
    marginBottom: 20
  },
  grid: {
    paddingBottom: 160 // מרווח גדול כדי שהתמונות האחרונות לא יוסתרו
  },
  imageContainer: {
      margin: 5,
      borderRadius: 8,
      overflow: 'hidden',
      backgroundColor: '#333',
      borderWidth: 1,
      borderColor: '#444'
  },
  imageThumbnail: {
    width: 100,
    height: 100,
  },
  
  // עיצוב ה-Modal
  modalBackground: {
      flex: 1,
      backgroundColor: 'rgba(0,0,0,0.9)',
      justifyContent: 'center',
      alignItems: 'center',
  },
  modalContent: {
      width: '90%',
      backgroundColor: '#222',
      borderRadius: 15,
      padding: 20,
      alignItems: 'center',
      borderWidth: 1,
      borderColor: '#444'
  },
  fullImage: {
      width: '100%',
      height: 400,
      marginBottom: 20,
      borderRadius: 10,
  },
  modalButtons: {
      flexDirection: 'row',
      justifyContent: 'space-between',
      width: '100%',
      gap: 10
  },
  closeBtn: {
      flex: 1,
      backgroundColor: '#03DAC6',
      padding: 12,
      borderRadius: 10,
      alignItems: 'center',
  },
  deleteBtn: {
      flex: 1,
      backgroundColor: '#FF5252',
      padding: 12,
      borderRadius: 10,
      alignItems: 'center',
  },

  // עיצוב ה-Footer והכפתורים (מיקום מתוקן)
  footer: {
    position: 'absolute',
    bottom: 50, // מורם למעלה מעל ה-Navigation Bar
    left: 20,
    right: 20,
    gap: 12, 
    backgroundColor: 'rgba(18, 18, 18, 0.95)', // רקע כמעט אטום כדי שהטקסט יהיה קריא מעל תמונות
    padding: 15,
    borderRadius: 15,
    borderWidth: 1,
    borderColor: '#333'
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