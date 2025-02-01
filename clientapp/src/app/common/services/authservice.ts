
import agent from "../api/agent";
import apiRoutes from "../api/apiRoutes";
import GenResponse, { StatusCode } from "../config/GenResponse";
import { AuthProfile } from "../models/AuthProfile";
import { AuthRegister } from "../models/AuthRegister";
import AuthUserLoginDto from "../models/AuthUserLoginDto";
// import { UserLoginResponse } from "../models/UserLoginResponse";
import { appServices } from "./appservices";

export class AuthService{


    async logout(): Promise<boolean> {
        let objResp : boolean = false;
        return new Promise((resolve, reject) => {
            agent.requests.post<boolean>(apiRoutes.auth.logout, {})
            .then((resp)=> {
                objResp = resp;
                resolve(objResp);
            })
            .catch((err) => {
                appServices.commonService.LogError("Error in logging out: ", err);
                objResp = false;
                reject(objResp);
            });
        });
    }

    async login(userLogin: AuthUserLoginDto): Promise<GenResponse<AuthProfile>> {
        let objResp : GenResponse<AuthProfile> = new GenResponse();
        return new Promise((resolve, reject) => {
            agent.requests.post<GenResponse<AuthProfile>>(apiRoutes.auth.login, userLogin)
            .then((resp)=> {
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

    async SocialLoginWithRegister(userProfile: AuthRegister): Promise<GenResponse<AuthProfile>> {
        let objResp : GenResponse<AuthProfile> = { result: {} as AuthProfile, isSuccess: false, message: "", statCode: StatusCode.OK, error: null };
        return new Promise((resolve, reject) => {
            agent.requests.post<GenResponse<AuthProfile>>(apiRoutes.auth.socialRegisterWithLogin, userProfile)
            .then((resp)=> {
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