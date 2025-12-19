type Listener = (msg: any) => void;

class WebSocketService {
  private socket: WebSocket | null = null;
  private listeners: Listener[] = [];
  private url: string = '';
  private token: string = '';
  
  // דגל שמונע חיבור מחדש אם המשתמש התנתק בכוונה (למשל Logout)
  private isExplicitlyDisconnected = false; 
  private reconnectTimeout: NodeJS.Timeout | null = null;

  constructor() {
    // הבנאי ריק כי אנחנו רוצים שליטה ידנית על ה-connect
  }

  connect(url: string, token: string) {
    // מניעת חיבור כפול אם כבר מחוברים
    if (this.socket && (this.socket.readyState === WebSocket.OPEN || this.socket.readyState === WebSocket.CONNECTING)) {
        console.log("🔌 WS Service: Already connected (Singleton check).");
        return;
    }

    console.log("🔌 WS Service: Starting connection..."); 
    this.url = url.split('?')[0]; 
    this.token = token;
    this.isExplicitlyDisconnected = false;
    this.initConnection();
  }

  private initConnection() {
    if (this.isExplicitlyDisconnected) return;

    const fullUrl = `${this.url}?jwt=${this.token}`;
    this.socket = new WebSocket(fullUrl);

    this.socket.onopen = () => {
      console.log("✅ WS Service: Connected!");
      
      // אם היה טיימר לחיבור מחדש - ננקה אותו כי הצלחנו
      if (this.reconnectTimeout) {
          clearTimeout(this.reconnectTimeout);
          this.reconnectTimeout = null;
      }
    };

    this.socket.onmessage = (event) => {
      try {
        const parsed = JSON.parse(event.data);
        // לוג מינימלי ונקי
        // console.log(`📩 WS Received: ${parsed.type || 'unknown type'}`); 
        this.notifyListeners(parsed);
      } catch (e) {
        console.error("❌ WS Error handling message:", e);
      }
    };

    this.socket.onerror = (e: any) => {
        if (!this.isExplicitlyDisconnected) {
            console.error("❌ WS Error detected.");
        }
    };

    this.socket.onclose = (event) => {
      if (this.isExplicitlyDisconnected) {
          console.log("ℹ️ WS Disconnected by user.");
          return;
      }

      console.warn(`⚠️ WS Closed unexpectedly (Code: ${event.code}). Reason: ${event.reason || 'None'}`);

      // מנגנון חיבור מחדש אוטומטי
      if (!this.reconnectTimeout) {
          console.log("🔄 Reconnecting in 3s...");
          this.reconnectTimeout = setTimeout(() => {
              this.reconnectTimeout = null; // איפוס הטיימר לפני הניסיון
              this.initConnection();       // <--- התיקון החשוב: זה כבר לא בהערה
          }, 3000);
      }
    };
  }

  subscribe(listener: Listener) {
    this.listeners.push(listener);
    
    // החזרת פונקציית Unsubscribe נקייה
    return () => {
      this.listeners = this.listeners.filter((l) => l !== listener);
    };
  }

  private notifyListeners(msg: any) {
    this.listeners.forEach((l) => l(msg));
  }

  // השתמש בזה *רק* בלחיצה על Logout
  disconnect() { 
    console.log("🧨 WS Service: Disconnecting explicitly...");
    this.isExplicitlyDisconnected = true;
    
    if (this.reconnectTimeout) {
        clearTimeout(this.reconnectTimeout);
        this.reconnectTimeout = null;
    }

    if (this.socket) {
        this.socket.close(); // סגירה נקייה (1000)
        this.socket = null;
    }
  }
}

// ייצוא מופע יחיד (Singleton) לשימוש בכל האפליקציה
export const socketService = new WebSocketService();