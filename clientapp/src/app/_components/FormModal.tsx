import { FormEvent, JSX} from 'react';
import { useDispatch } from 'react-redux';
import Dynamic from '../common/utils/Dynamic';
import { UserLoginResponseUpdateDTO } from '../common/models/UserLoginResponse';
import { setLoading } from '../../store/slices/common/Common.slice';

export interface IFormModalProps <T>{
    table: "userProfile" | "projects";
    type: "create" | "update" | "delete";
    id?: string;
    title?: string;
    data?: T;
    description?: string;
    open: boolean;
    onClose: () => void;
}

// Code splitting
const ProfileForm = Dynamic<IFormModalProps<UserLoginResponseUpdateDTO>>(() => import('./forms/ProfileForm'),{fallback: <div>Loading...</div>});



const forms: {
    [key: string] :(type: "create" | "update" , data?: object) => JSX.Element;
}={
    userProfile: (type, data) => <ProfileForm table='userProfile' type={type} data={data as UserLoginResponseUpdateDTO} open={false} onClose={function (): void {
        throw new Error('Function not implemented.');
    } }/>,
}


const FormModal =<T extends object> ({table, type, id, data, open, onClose}: IFormModalProps<T>) => {

    const dispatch = useDispatch();
    // const size = type === "create" ? "w-8 h-8" : "w-7 h-7";
    // const bgColor = type === "create" ? "bg-onaxYellow" : type === "update" ? "bg-onaxSky" : "bg-onaxPurple";
    // const image = type === "create"? "plus" : type === "update" ? "edit" : "delete";
    // const imgtitle = title ? title : '';

    // const [open, setOpen] = React.useState(false);

    const HandleSubmit = (evt: FormEvent) => {
        evt.preventDefault();
        console.log('Deleting teacher with id: ', id);
        if(type === "delete" && id){
            if(table === "userProfile"){   
                dispatch(setLoading({display: true, message: `Deleting ${table}...`}));
                //TODO: Remove time out and implement delete logic. Only used for similating API call
                setTimeout(() => {
                    // dispatch(deleteTeacher(id!));
                    dispatch(setLoading({display: false, message: ''}));
                },1200);
            }else{
                alert('Currently, only teachers can be deleted');
                onClose();
            }
        }
        if(type === "create"){
            if(table === "userProfile" && data){
                console.log('Creating new record');
                
            }
        }
    }

    // const HandleClick=(evt: React.MouseEvent<HTMLElement>)=>{
    //     setOpen(true);
    //     const dataVal = evt.currentTarget.getAttribute('data-val');
    //     if(dataVal){
    //         console.log(JSON.stringify(dataVal));
    //     }
    // }

    const FormControl = ()=>{
         return type === "delete" && id ? (
            <form action='' onSubmit={HandleSubmit} className='p-4 flex flex-col gap-4'>
                <span className='text-center font-medium'>Confirm {table} delete?</span>
                <button className='font-semibold bg-red-500 rounded-md w-max px-4 py-2 text-onaxOffWhite border-none self-center'>Delete</button>
            </form>
        ): type === "create" || type === "update" ? (forms[table](type, data)) 
        : <div className='text-red-400'>Form not found!</div>;
    };

  return (
    <>
        {/* <div className={`${size} flex items-center justify-center rounded-full ${bgColor} cursor-pointer`} data-val={data}>
            <img src={`/assets/images/${image}.png`} className='bg-green-500' alt="" title={`${imgtitle}`} width={16} height={16}/>
            <span>{description}</span>
        </div> */}
        {open &&(
            <div className="w-screen h-screen absolute top-0 left-0 bg-black bg-opacity-40 z-50 flex items-center justify-center">
                <div className="bg-white p-4 rounded-md relative w-[90%] md:w-[70%] lg:w-[50%]">
                    {FormControl()}
                    <div className="absolute top-4 right-4 cursor-pointer" onClick={onClose}>
                        <img src="/assets/images/close.png" alt="Close" title="Close" width={14} height={14} />
                    </div>
                </div>
            </div>
        )}
    </>
  )
}

export default FormModal;