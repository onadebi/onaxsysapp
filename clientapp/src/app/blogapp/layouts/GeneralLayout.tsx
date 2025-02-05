import React, { useLayoutEffect } from "react";
import { Outlet } from "react-router-dom";
import NavBar from "./includes/navbar/NavBar";
import { useDispatch } from "react-redux";
import { setAuthProfile } from "../../../store/slices/authSlice";
import { AppDispatch } from "../../../store/storeKeeper";
import GenResponse from "../../common/config/GenResponse";
import { useAppStore } from "../../common/services/appservices";
import appsettings from "../../common/config/appsettings";

const GeneralLayout: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();

  const {authService,commonService} = useAppStore();
  useLayoutEffect(() => {
    authService.IsSignedIn()
    .then((resp: GenResponse<boolean>) => {
      if (resp && resp.isSuccess && resp.result) {
        const userProf = authService.UserProfile()?.result;
        if(userProf !== null && userProf !== undefined){
          dispatch(setAuthProfile(userProf));
        }
      }else{
        commonService.removeLocalStorage(appsettings.token.authToken);
      }
    })
    .catch((err: GenResponse<boolean>) => {console.error(`error: ${err.result}`);});
  }, [authService, commonService, dispatch]);

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
