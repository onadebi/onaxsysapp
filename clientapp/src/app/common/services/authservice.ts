import agent from "../api/agent";
import apiRoutes from "../api/apiRoutes";
import GenResponse, { StatusCode } from "../config/GenResponse";
import { AuthRegister } from "../models/AuthRegister";
import AuthUserLoginDto from "../models/AuthUserLoginDto";
import { jwtDecode } from "jwt-decode";
import { appServices } from "./appservices";
import { UserLoginResponse } from "../models/UserLoginResponse";
import appsettings from "../config/appsettings";
import { UserLoginTokenPayload } from "../models/UserLoginTokenPayload";

export class AuthService {
    async logout(): Promise<boolean> {
        let objResp: boolean = false;
        return new Promise((resolve, reject) => {
            agent.requests.post<boolean>(apiRoutes.auth.logout, {})
                .then((resp) => {
                    objResp = resp;
                    appServices.commonService.removeLocalStorage(appsettings.token.authToken);
                    resolve(objResp);
                })
                .catch((err) => {
                    appServices.commonService.LogError("Error in logging out: ", err);
                    objResp = false;
                    reject(objResp);
                });
        });
    }

    async login(userLogin: AuthUserLoginDto): Promise<GenResponse<UserLoginResponse>> {
        let objResp: GenResponse<UserLoginResponse> = new GenResponse();
        return new Promise((resolve, reject) => {
            agent.requests.post<GenResponse<UserLoginResponse>>(apiRoutes.auth.login, userLogin)
                .then((resp) => {
                    objResp = resp;
                    resolve(objResp);
                })
                .catch((err) => {
                    appServices.commonService.LogError("Error in logging in: ", err);
                    objResp.result = null;
                    reject(objResp);
                });
        });
    }

    async IsSignedIn(tokenName = 'onx_token'): Promise<GenResponse<boolean>> {
        const objResp: GenResponse<boolean> = new GenResponse();
        return new Promise((resolve, reject) => {
            const token = localStorage.getItem(tokenName);
            if (token) {
                //TODO: validate token Expiry and remove from local storage if expired
                const tokenPayload = JSON.parse(token) as GenResponse<UserLoginResponse>;
                let decodedToken: UserLoginTokenPayload | null = null;
                if (tokenPayload.result?.token) {
                    decodedToken = jwtDecode(tokenPayload.result.token) as UserLoginTokenPayload;
                    //Check if the token has expired
                    const currentDateTime = Date.now() / 1000;
                    if (decodedToken?.exp && decodedToken.exp > currentDateTime) {
                        objResp.result = objResp.isSuccess = true;
                    } else {
                        objResp.error = objResp.message = 'Invalid token.';
                        appServices.commonService.removeLocalStorage(appsettings.token.authToken);
                        objResp.result = false;
                    }
                } else {
                    objResp.error = objResp.message = 'Invalid token.';
                    objResp.result = false;
                    appServices.commonService.removeLocalStorage(appsettings.token.authToken);
                }
                resolve(objResp);
            } else {
                objResp.result = false;
                objResp.error = 'Token not found';
                reject(objResp);
            }
        });
    }

    UserProfile(tokenName = 'onx_token'): GenResponse<UserLoginResponse | null> {
        let objResp: GenResponse<UserLoginResponse | null> = new GenResponse();
        const token = localStorage.getItem(tokenName);
        try {
            if (token) {
                const tokenPayload = JSON.parse(token) as GenResponse<UserLoginResponse>;
                let decodedToken: UserLoginTokenPayload | null = null;
                if (tokenPayload.result?.token) {
                    decodedToken = jwtDecode(tokenPayload.result.token) as UserLoginTokenPayload;
                    //Check if the token has expired
                    const currentDateTime = Date.now() / 1000;
                    if (decodedToken?.exp && decodedToken.exp > currentDateTime) {
                        objResp = tokenPayload;
                        objResp.isSuccess = true;
                    } else {
                        objResp.error = objResp.message = 'Invalid token.';
                        appServices.commonService.removeLocalStorage(appsettings.token.authToken);
                        objResp.result = null;
                    }
                }
                else {
                    objResp.error = objResp.message = 'Invalid token.';
                    objResp.result = null;
                    appServices.commonService.removeLocalStorage(appsettings.token.authToken);
                }
            } else {
                objResp.result = null;
                objResp.isSuccess = false;
            }
        }
        catch (er) {
            appServices.commonService.LogError('Error fetching token:', er as Error);
        }
        return objResp;
    }

    async SocialLoginWithRegister(userProfile: AuthRegister): Promise<GenResponse<UserLoginResponse>> {
        let objResp: GenResponse<UserLoginResponse> = { result: {} as UserLoginResponse, isSuccess: false, message: "", statCode: StatusCode.OK, error: null };
        return new Promise((resolve, reject) => {
            agent.requests.post<GenResponse<UserLoginResponse>>(apiRoutes.auth.socialRegisterWithLogin, userProfile)
                .then((resp) => {
                    objResp = resp;
                    resolve(objResp);
                })
                .catch((err) => {
                    appServices.commonService.LogError("Error in SocialLoginWithRegister API call: ", err);
                    objResp.error = err;
                    reject(objResp);
                });
        });
    }
}