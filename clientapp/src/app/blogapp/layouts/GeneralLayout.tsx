import React from "react";
import { Outlet } from "react-router-dom";
import NavBar from "./includes/navbar/NavBar";
import { useAuth, useUser } from "@clerk/clerk-react";
import { useSelector, useDispatch } from "react-redux";
import { setAuthProfile } from "../../../store/slices/authSlice";
import { RootState, AppDispatch } from "../../../store/storeKeeper";
import GenResponse from "../../common/config/GenResponse";
import { AuthProfile } from "../../common/models/AuthProfile";
import AuthUserLoginDto from "../../common/models/AuthUserLoginDto";
import { appServices } from "../../common/services/appservices";

const GeneralLayout: React.FC = () => {
  const userProfile = useSelector((state: RootState) => state.auth);
  const dispatch = useDispatch<AppDispatch>();

  const { isSignedIn, getToken, } = useAuth();
  const { user } = useUser();


  if (isSignedIn) {
      getToken().then((token) => {
       // First check in redux store if user is already logged in
       if(!userProfile || !userProfile?.email){
         //next check in local storage
         const userProf: (AuthProfile| null) = appServices.commonService.getSessionCookieUserProfileToBtoa();
         if(userProf){
           try{
             dispatch(setAuthProfile(userProf));
           }
           catch(er){
             appServices.commonService.LogError('Error fetching token:', er as Error);
           }
         }
         else{
           const authProfileLogin: AuthUserLoginDto ={
             email: user?.primaryEmailAddress?.emailAddress ? user?.primaryEmailAddress?.emailAddress : "",
             password: "",
             socialLogin: {
               isSocialLogin: true,
               socialLoginAppName: "Clerk",
               app_id: user?.id ? user?.id : "",
               token: token ? token : ""
             }
           }
           appServices.authService.login(authProfileLogin)
             .then((response: GenResponse<AuthProfile>) => {
               // appServices.commonService.setSessionCookieUserProfileToBtoa(response.result);
               if(response.isSuccess && response.result) {
                 dispatch(setAuthProfile(response.result));                 
               }               
             }).catch((error) => {
               appServices.commonService.LogError('Error fetching token:', error);
             });
         }
       }
     }).catch((error) => {
       appServices.commonService.LogError('Error fetching token:', error);
     });
  }

  return (
    <>
      <div>
        <NavBar />
        <div className="px-4 md:px-8 lg:px-16 lx:px-28 2xl:px-36">
          <Outlet />
        </div>
      </div>
    </>
  );
};

export default GeneralLayout;
