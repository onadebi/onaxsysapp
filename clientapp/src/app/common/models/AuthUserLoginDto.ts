import { SocialLogin } from "./UserLoginResponse";

export default class AuthUserLoginDto {
    email!: string;
    password!: string;
    socialLogin!: SocialLogin;
}