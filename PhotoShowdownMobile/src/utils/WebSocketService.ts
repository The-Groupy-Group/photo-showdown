type Listener = (msg: any) => void;

class WebSocketService {
  private socket: WebSocket | null = null;
  private listeners: Listener[] = [];
  private url: string = '';
  private token: string = '';
  
  private isExplicitlyDisconnected = false; 
  private reconnectTimeout: NodeJS.Timeout | null = null;

  constructor() {
  }

  connect(url: string, token: string) {
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
    
      if (this.reconnectTimeout) {
          clearTimeout(this.reconnectTimeout);
          this.reconnectTimeout = null;
      }
    };

   this.socket.onmessage = (event) => {
      try {
        const parsed = JSON.parse(event.data);
        
        const rawType = parsed.type || parsed.Type;
        const msgTypeLower = (typeof rawType === 'string') 
            ? rawType.toLowerCase() 
            : rawType;

        const isMatchEnded = msgTypeLower === 'matchended' || msgTypeLower === 6;

        if (isMatchEnded) {
             console.log("🏁 Match Ended received. Preparing for graceful shutdown.");
             this.isExplicitlyDisconnected = true;
        }

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

      if (!this.reconnectTimeout) {
          console.log("🔄 Reconnecting in 3s...");
          this.reconnectTimeout = setTimeout(() => {
              this.reconnectTimeout = null; 
              this.initConnection();
          }, 3000);
      }
    };
  }

  subscribe(listener: Listener) {
    this.listeners.push(listener);
    
    return () => {
      this.listeners = this.listeners.filter((l) => l !== listener);
    };
  }

  private notifyListeners(msg: any) {
    this.listeners.forEach((l) => l(msg));
  }

  disconnect() { 
    console.log("🧨 WS Service: Disconnecting explicitly...");
    this.isExplicitlyDisconnected = true;
    
    if (this.reconnectTimeout) {
        clearTimeout(this.reconnectTimeout);
        this.reconnectTimeout = null;
    }

    if (this.socket) {
        this.socket.close();
        this.socket = null;
    }
  }
}

export const socketService = new WebSocketService();