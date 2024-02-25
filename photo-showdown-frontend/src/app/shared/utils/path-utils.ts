import { environment } from "src/environments/environment";

export class UrlUtils {

    static getBasePicturesURL() {
        return `${environment.apiUrl.replace("api","pictures")}`;
    }
}