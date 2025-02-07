
export type UserLoginResponse = {
    firstName: string;
    lastName: string;
    email: string;
    guid: string;
    id: string;
    roles: string[];
    token?: string;
    socialLogin?: SocialLogin;
}

export interface SocialLogin {
    isSocialLogin: boolean
    socialLoginAppName: string
    app_id: string;
    token: string
}

export type UserLoginResponseUpdateDTO =  Omit<UserLoginResponse,'socialLogin'|'token'|'roles'> & {
    sex: 'male' | 'female'| 'unspecified';
}
  
export type UserAccountCreateDTO = Omit<UserLoginResponse,'socialLogin'|'token'|'roles'|'guid'|'id'> & {
    password: string;
    confirmPassword: string;
}