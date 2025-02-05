import appsettings from "../config/appsettings";
import { UserLoginResponse } from "../models/UserLoginResponse";


export default class CommonService {

    error: string | null = null;
    token: string | null = window.localStorage.getItem('jwt');
    appLoaded = false;

    LogError = (error: string, obj?: object | string | undefined) => {
        if (obj) {
            console.error(`[${appsettings.appName}] ERROR :`, error, obj);
        } else {
            console.error(`[${appsettings.appName}] ERROR :`, error);
        }
    }

    LogActivity = (prop: string | object, obj?: object | undefined) => {
        if (obj) {
            console.log(`[${appsettings.appName}] INFO :`, prop, obj);
        } else {
            console.info(`[${appsettings.appName}] INFO :`, prop);
        }
    }

    setServerError = (error: string) => {
        this.error = error;
    }

    setLocaStoragae = (tokenKey: string, tokenValue: string) =>(window.localStorage.setItem(tokenKey, tokenValue));
    removeLocalStorage = (tokenKey: string) =>(window.localStorage.removeItem(tokenKey));

    // Function to set a session cookie
    setSessionCookie = (name: string, value: string) => {
        window.document.cookie = `${name}=${value}; path=/`;
    }

    setSessionCookieUserProfileToBtoa = (userProf: UserLoginResponse, userSession: string = '_onx_appuser_session') => {
        window.document.cookie = `${userSession}=${btoa(JSON.stringify(userProf))}; path=/`;
    }

    // Function to retrieve a session cookie by name
    getSessionCookie = (name: string) => {
        const cookieArr = window.document.cookie.split("; ");
        for (const cookie of cookieArr) {
            const [key, value] = cookie.split("=");
            if (key === name) return value;
        }
        return null;
    }

    getSessionCookieUserProfileToBtoa = (userSession: string = '_onx_appuser_session'): (UserLoginResponse | null) => {
        let decoded: string;
        //TODO: Local JWT decode validation (expired, tampered, etc)
        const cookieArr = window.document.cookie.split("; ");
        for (const cookie of cookieArr) {
            const [key, value] = cookie.split("=");
            if (key === userSession) {
                decoded = atob(value);
                const authProfile = decoded as unknown as UserLoginResponse;
                return authProfile;//JSON.parse(authProfile);
            }
        }
        return null;
    }

    // fetchValidLocalAuthProfileFromBtoa = (token: string = 'token'): (UserLoginResponse | null) => {
    //     const userAuth = window.localStorage.getItem(token);
    //     let decoded: string;
    //     //TODO: Local JWT decode validation (expired, tampered, etc)
    //     if (userAuth) {
    //         decoded = ""; //atob(userAuth);
    //         const authProfile = decoded;//as unknown as UserLoginResponse;
    //         return JSON.parse(authProfile);
    //     } else {
    //         return null;
    //     }
    // }

    setLocalAuthProfileToBtoa = (userProf: UserLoginResponse, token: string = 'token') => {
        window.localStorage.setItem(token, btoa(JSON.stringify(userProf)));
    }

    fetchFromLocalBtoa = (key: string) => {
        const value = window.localStorage.getItem(key);
        if (value) {
            return atob(value);
        }
        return null;
    }


    setToLocalBtoa = (key: string, value: string) => {
        window.localStorage.setItem(key, btoa(value));
    }

    setAppLoaded = () => {
        this.appLoaded = true;
    }

}