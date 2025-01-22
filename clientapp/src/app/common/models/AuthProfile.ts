export type AuthProfile  ={
    firstName: string;
    lastName: string;
    email: string;
    guid: string;
    id: number;
    roles: string[];
    token: string;
    socialLogin: SocialLogin;
  }
  
  export interface SocialLogin {
    isSocialLogin: boolean
    socialLoginAppName: string
    app_id: string;
    token: string
  }
  