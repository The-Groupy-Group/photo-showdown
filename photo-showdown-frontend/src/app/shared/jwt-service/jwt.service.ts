
import { Injectable } from '@angular/core';
import jwt_decode from "jwt-decode";
@Injectable({
  providedIn: 'root'
})
export class JwtService {

  constructor() { }
  /**
   *decodes the token_id from local storage
   * @returns string of the user's id
   */
  getTokenId(): string | undefined{
    const idToken = localStorage.getItem("id_token");
    if (idToken)
    {
      const decodedToken: { Id?: string } = jwt_decode(idToken);
      if (decodedToken && 'Id' in decodedToken)
      {
        return decodedToken.Id;
      }
    }
    return undefined;
  }
}
