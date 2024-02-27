import { environment } from 'src/environments/environment';

/**
 * Utility class for URL related operations
 */
export abstract class UrlUtils {
  /**
   * Returns the base URL for the pictures, build from the environment.apiUrl
   * @param pictureName The name of the picture
   * @returns The base URL for the pictures
   */
  static getPictureURL(pictureName: string) {
    return `${environment.apiUrl.replace('api', 'pictures')}/${pictureName}`;
  }

  static getWebSocketUrl() {
    return environment.apiUrl.replace('http', 'ws') + '/ws';
  }
}
