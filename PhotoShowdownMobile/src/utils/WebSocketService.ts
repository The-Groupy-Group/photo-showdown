// src/utils/WebSocketService.ts

type MessageListener = (message: any) => void;

class WebSocketService {
  private static instance: WebSocketService;
  private socket: WebSocket | null = null;
  private listeners: MessageListener[] = [];
  private isExplicitlyDisconnected: boolean = false;
  private reconnectTimeout: NodeJS.Timeout | null = null;

  public static getInstance(): WebSocketService {
    if (!WebSocketService.instance) {
      WebSocketService.instance = new WebSocketService();
    }
    return WebSocketService.instance;
  }

  // שים לב: הפונקציה מקבלת רק URL וטוקן (בלי matchId)
  public connect(baseUrl: string, token: string) {
    if (this.socket && (this.socket.readyState === WebSocket.OPEN || this.socket.readyState === WebSocket.CONNECTING)) {
      console.log("Socket already connected");
      return;
    }

    this.isExplicitlyDisconnected = false;
    
    // --- התאמה ל-Backend שלך ---
    // השרת מצפה לפרמטר בשם ?jwt=...
    const fullUrl = `${baseUrl}?jwt=${token}`;
    
    console.log("Connecting to WebSocket:", fullUrl);
    this.socket = new WebSocket(fullUrl);

    this.setupListeners();
  }

  private setupListeners() {
    if (!this.socket) return;

    this.socket.onopen = () => {
      console.log("✅ WebSocket Connected Successfully!");
    };

    this.socket.onmessage = (event) => {
      try {
        const parsedData = JSON.parse(event.data);
        console.log("📩 New Message:", parsedData);
        this.listeners.forEach((listener) => listener(parsedData));
      } catch (e) {
        console.error("Error parsing message:", event.data);
      }
    };

    this.socket.onerror = (error) => {
      console.error("❌ WebSocket Error:", error);
    };

    this.socket.onclose = (e) => {
      console.log("⚠️ WebSocket Closed", e.code, e.reason);
      if (!this.isExplicitlyDisconnected) {
         this.handleReconnection(this.socket?.url || "");
      }
    };
  }

  public sendMessage(type: string, payload: any) {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      const message = JSON.stringify({ type, ...payload });
      this.socket.send(message);
    } else {
      console.warn("Cannot send message, socket is not open");
    }
  }

  public subscribe(listener: MessageListener) {
    this.listeners.push(listener);
    return () => {
      this.listeners = this.listeners.filter((l) => l !== listener);
    };
  }

  public disconnect() {
    this.isExplicitlyDisconnected = true;
    if (this.socket) {
      this.socket.close();
      this.socket = null;
    }
    if (this.reconnectTimeout) {
      clearTimeout(this.reconnectTimeout);
    }
  }

  private handleReconnection(lastUrl: string) {
      if (this.reconnectTimeout) return;
      
      console.log("🔄 Attempting to reconnect in 3 seconds...");
      this.reconnectTimeout = setTimeout(() => {
          this.reconnectTimeout = null;
          if (!this.isExplicitlyDisconnected && lastUrl) {
             console.log("Reconnecting...");
             this.socket = new WebSocket(lastUrl);
             this.setupListeners();
          }
      }, 3000);
  }
}

export default WebSocketService.getInstance();