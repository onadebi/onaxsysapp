import React from 'react'
import 'react-quill-new/dist/quill.snow.css';
import { Outlet, useNavigate } from 'react-router-dom';
import NavBar from './includes/navbar/NavBar';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../../../store/storeKeeper';
import { useAuth, useUser } from '@clerk/clerk-react';
import LoaderGeneric from '../../_components/LoaderGeneric';
import { appServices } from '../../common/services/appservices';
import { setAuthProfile } from '../../../store/slices/authSlice';
import { AuthProfile } from '../../common/models/AuthProfile';
import GenResponse from '../../common/config/GenResponse';
import BlogAppRoutes from '../BlogAppRoutes';
import AuthUserLoginDto from '../../common/models/AuthUserLoginDto';
import RouteTo from '../../_components/RouteTo';

const DashboardCards: React.FC<{ title: string; url: string }> = ({
  title,
  url,
}) => {
  return (
    <>
      <div className="bg-white shadow-md rounded-md p-4 min-w-[200px]">
        <RouteTo to={url} className="text-blue-800">
          <h3 className="text-lg font-semibold">{title}</h3>
        </RouteTo>
      </div>
    </>
  );
};

const permittedRoles = ['admin'];
const operations = [
  {
    title: "Home",
    url: BlogAppRoutes().dashboard.home.parentRoute,
    order: 0,
  },
  {
    title: "Create Post",
    url: BlogAppRoutes().dashboard.write.parentRoute,
    order: 1,
  },
];

const DashboardLayout: React.FC = () => {

  const navigate = useNavigate();
  const [loading, setLoading] = React.useState(true);
  const userProfile = useSelector((state: RootState) => state.auth);
  const dispatch = useDispatch<AppDispatch>();

  const { isSignedIn, isLoaded, getToken } = useAuth();// as { isSignedIn: boolean, isLoaded: boolean, getToken: () => Promise<string> };
  const { user } = useUser();

  React.useEffect(() => {
    if (!isLoaded) return;
    if (!isSignedIn) {
      navigate(BlogAppRoutes().public.login.parentRoute);
      return;
    }
    getToken().then((token) => {
      console.log('Token::::=>', {name: `${user?.firstName} ${user?.lastName}`,email: user?.primaryEmailAddress?.emailAddress, imageUrl: user?.hasImage ? user?.imageUrl : null, token: token});
      // First check in redux store if user is already logged in
      if(!userProfile || !userProfile?.email || !userProfile?.roles){
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
          console.log('api call for social login');
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
              if(response.isSuccess && response.data) {
                dispatch(setAuthProfile(response.data));
                if(!response.data.roles.some(r => permittedRoles.includes(r))){
                  alert('You do not have permission to access this page');
                  navigate(BlogAppRoutes().public.home.parentRoute);
                }
              }
              else{
                appServices.commonService.LogError(`Error fetching token: ${response?.error}`,);
                alert('Error logging in');
                navigate(BlogAppRoutes().public.login.parentRoute);
              }
              
            }).catch((error) => {
              appServices.commonService.LogError('Error fetching token:', error);
              navigate(BlogAppRoutes().public.login.parentRoute);
            }).finally(() => {
              setLoading(false);
            });
        }
      }else{
        if(!userProfile.roles.some(r => permittedRoles.includes(r))){
          alert('You do not have permission to access this page');
          navigate(BlogAppRoutes().public.home.parentRoute);
        }
      }
    }).finally(()=> setLoading(false)).catch((error) => {
      appServices.commonService.LogError('Error fetching token:', error);
    });
  }, [isLoaded, isSignedIn, getToken, user, userProfile, dispatch, navigate]);

  return (
    <>
    <div>
      {loading ? <LoaderGeneric display={true}/> : 
        <>
        <NavBar />
        <h3>DashboardLayout</h3>
        <div className="px-4 md:px-8 lg:px-4 lx:px-28 2xl:px-6">
          <div className='flex'>
            <aside className='hidden md:flex flex-col gap-4 w-1/8 2xl:mr-2 lx:mr-2 lg:mr-2 md:mr-2'>
            {
              operations.map((op, index) => (
                <DashboardCards key={index} title={op.title} url={op.url} />
              ))
            }
            </aside>
            <div className='w-full p-1'>
              <Outlet />
            </div>
          </div>
        </div>
        </>
      }
    </div>
  </>
  )
}

export default DashboardLayout;