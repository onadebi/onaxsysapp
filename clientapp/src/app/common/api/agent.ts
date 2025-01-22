import axios, { AxiosError, AxiosResponse } from 'axios';
import appsettings from '../config/appsettings';
// import swal from 'sweetalert';

if(appsettings.env && appsettings.env.toLowerCase().includes('dev')){
    axios.defaults.baseURL = `${appsettings.baseUrls.baseUrl}`;
}else{
    axios.defaults.baseURL = appsettings.baseUrls.baseUrl;
}

axios.defaults.withCredentials = true;
// Use interceptors for detection of 401, so as to invalidate Token and clear from local storage and redirect to login
axios.interceptors.response.use(resp => {
    resp.headers['Access-Control-Allow-Origin'] = '*';
    return resp;
}, (error: AxiosError) => {
    console.log(JSON.stringify(error,null, 2));
    // const { status, data } = error.response as AxiosResponse<object>;
    const ex = error;// as AxiosResponse<object>;
    console.log(`Axios error status: ======>`, JSON.stringify(ex));
    // const { data } = error.response as AxiosResponse<object>;
    // console.log(`Axios error status: ${status} data: ${JSON.stringify(data)}`);
    // console.log(`Axios error status: ${`status`} data: ${JSON.stringify(data)}`);
    // switch (status) {
    //     case 401:
    //         swal({
    //             icon: 'error',
    //             title: 'Inactive session error!',
    //             buttons: {
    //                 Cancel: {
    //                     text: 'Cancel',
    //                     value: null,
    //                     closeModal: true
    //                 },
    //                 Ok: {
    //                     text: 'Ok',
    //                     value: true,
    //                     closeModal: true
    //                 }
    //             },
    //             text: 'You have been logged out due to inactivity for a prolonged period. Please login again to continue.',
    //         }).then((shouldWait) => {
    //             if (shouldWait) {
    //                 // storeService.userService.logout();
    //                 console.log('User ::', shouldWait);
    //             }
    //         });
    //         break;
    //     default:
    //         break;
    // }
});


const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T>(url: string) => axios.get<T>(`${url}`).then(responseBody),
    post: <T>(url: string, body: object) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: object) => axios.put<T>(url, body).then(responseBody),
    delete: <T>(url: string) => axios.delete<T>(`${url}`).then(responseBody),
}

const agent ={
    axios,
    requests,
}

export default agent;
