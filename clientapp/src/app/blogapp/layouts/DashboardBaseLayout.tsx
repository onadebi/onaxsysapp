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

// const permittedRoles = ['admin'];
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
        <h3>DashboardBaseLayout</h3>
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

export default DashboardBaseLayout