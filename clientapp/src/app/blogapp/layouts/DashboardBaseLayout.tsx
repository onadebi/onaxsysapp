import React, { useLayoutEffect } from 'react';
import NavBar from './includes/navbar/NavBar';
import LoaderGeneric from '../../_components/LoaderGeneric';
import RouteTo from '../../_components/RouteTo';
import BlogAppRoutes from '../BlogAppRoutes';
import { Outlet, useNavigate } from 'react-router-dom';
// import { AppDispatch, RootState } from '../../../store/storeKeeper';
// import { useDispatch, useSelector } from 'react-redux';
import { useAppStore } from '../../common/services/appservices';
import GenResponse from '../../common/config/GenResponse';
import Menu from '../../_components/Menu';
import AppRoutes from '../../../routes/AppRoutes';


const DashboardBaseLayout: React.FC = () => {

  const navigate = useNavigate();
  const [loading, setLoading] = React.useState(true);
  // const userProfile = useSelector((state: RootState) => state.auth);
  // const dispatch = useDispatch<AppDispatch>();

  const {authService} = useAppStore();

  useLayoutEffect(() => {
    authService.IsSignedIn()
    .then((resp: GenResponse<boolean>) => {
          if (resp.result === false || resp.isSuccess === false) {
            navigate(BlogAppRoutes().public.login.parentRoute);
          }else{
            setLoading(false);
          }
        })
        .catch((err: GenResponse<boolean>) => {
          console.log(JSON.stringify(err));
          navigate(BlogAppRoutes().public.login.parentRoute);
        });
  });
  
  return (
    <>
    <div>
      {loading ? <LoaderGeneric display={true}/> : 
        <>
        <NavBar />
        {/* <div className="px-4 md:px-8 lg:px-4 lx:px-28 2xl:px-6"> */}
        <div className="">
          <div className='flex'>
              <aside className="w-[14%] md:w-[8%] lg:w-[16%] xl:w-[14%] bg-white h-[100vh] border-r-[#ececec] border-r-2 shadow-lg">
                <RouteTo to={AppRoutes().dashboard.home.parentRoute}
                  className="flex items-center justify-center lg:justify-start gap-2 p-4">
                  <img src="/assets/images/logo.png" alt="Logo" width={32} height={32} />
                  <span className="hidden lg:block">OnaxApp</span>
                </RouteTo>
                <Menu />
             </aside>
            {/* <aside className='hidden md:flex flex-col gap-4 w-1/8 2xl:mr-2 lx:mr-2 lg:mr-2 md:mr-2'>
            {
              operations.map((op, index) => (
                <DashboardCards key={index} title={op.title} url={op.url} />
              ))
            }
            </aside> */}
            <div className='w-full p-1 bg-slate-50'>
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

export default DashboardBaseLayout