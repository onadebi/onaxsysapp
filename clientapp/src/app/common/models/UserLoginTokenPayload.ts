export type UserLoginTokenPayload = {
    display_name: string;
    email: string;
    nameid: string[];
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid": string;
    role: string[];
    nbf: number;
    exp: number;
    iat: number;
}