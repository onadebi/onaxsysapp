import { createContext, useContext } from "react";
import CommonService from "./common-service";
import { AuthService } from "./authservice";
import NotificationsService from "./notificationsService";
import ImagesService from "./images-service";
import BlogAppService from "../../core/services/blogappService";

interface IAppServices {
    commonService: CommonService;
    authService: AuthService;
    notificationsService: NotificationsService;
    imagesService: ImagesService;
    blogService: BlogAppService;

    // #region Blog
    //#endregion
}


export const appServices: IAppServices = {
    commonService: new CommonService(),
    authService: new AuthService(),
    notificationsService: new NotificationsService(),
    imagesService: new ImagesService(),
    blogService: new BlogAppService()
};


export const AppContext = createContext<IAppServices>(appServices);

export const useAppStore =()=> useContext(AppContext);