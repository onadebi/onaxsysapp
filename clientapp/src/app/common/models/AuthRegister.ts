import { SocialLogin } from "./UserLoginResponse"

export type AuthRegister =  {
    firstName: string
    lastName: string
    email: string
    password: string
    confirmPassword: string
    UserProfileImage: string | null
    socialLogin: SocialLogin
  }