export interface JwtPayload {
	Id: string;
	Username: string;
	Roles: string[];
	exp: number;
	iss: string;
	aud: string;
}
