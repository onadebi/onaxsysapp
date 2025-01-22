import { SocialLogin } from "./AuthProfile";

export default class AuthUserLoginDto {
    email!: string;
    password?: string;
    socialLogin!: SocialLogin;
}