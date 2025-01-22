import { SocialLogin } from "./AuthProfile";

export class UserLoginResponse {
    firstName?: string;
    lastName?: string;
    email?: string | null;
    guid?: string;
    id: string = '';
    roles: string[] = [];
    token?: string;
    socialLogin?: SocialLogin;
}