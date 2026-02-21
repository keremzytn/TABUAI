import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.tabuai.app',
  appName: 'TabuAI',
  webDir: 'dist/tabu-ai-web',
  server: {
    // Android emülatör için: 10.0.2.2 host makinenin IP'sidir
    // iOS simulator ve gerçek cihazlar için local IP kullanılır
    cleartext: true, // HTTP bağlantılarına izin ver (development)
    allowNavigation: ['172.16.0.159:5092', '192.168.*', '10.0.2.2:5092']
  }
};

export default config;
