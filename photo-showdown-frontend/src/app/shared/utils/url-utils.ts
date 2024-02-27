import { environment } from 'src/environments/environment';

/**
 * Utility class for URL related operations
 */
export class UrlUtils {
    /**
     * Returns the base URL for the pictures, build from the environment.apiUrl
     * @param pictureName The name of the picture
     * @returns The base URL for the pictures
     */
  static getBasePicturesURL(pictureName: string) {
    return `${environment.apiUrl.replace('api', 'pictures')}/${pictureName}`;
  }
}
