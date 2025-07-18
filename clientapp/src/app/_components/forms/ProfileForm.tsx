import React from 'react'
import avatar from '../../../assets/images/avatar.png';
import { UserLoginResponseUpdateDTO } from '../../common/models/UserLoginResponse';
import FormInputField from '../FormInputField';
import camera from '../../../../public/assets/images/camera.svg';
import { RegInput, schema } from './schema';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { IPropsTableForm } from './IPropsTableForm';

const ProfileForm: React.FC<IPropsTableForm<UserLoginResponseUpdateDTO>> = ({type, data}) => {


  const fileInputRef = React.useRef<HTMLInputElement>(null);
  const [photoName, setPhotoName] = React.useState<string | null>(data?.picture ?? null);

  const {register,handleSubmit,formState: { errors },} = useForm <RegInput>({resolver: zodResolver(schema),});

    const OnSubmit = handleSubmit((data) => {
        console.log(data);
    });
    const handleImageClick = () => {
      if (fileInputRef.current) {
      fileInputRef.current.click();
      }
    };

    const handleImageChange = (evt: React.ChangeEvent<HTMLInputElement>) => {
      if (evt.target.files && evt.target.files.length > 0) {
      setPhotoName(evt.target.files[0].name);
      }
    };
  return (
    <>
    <form className="flex flex-col gap-8" onSubmit={OnSubmit}>
        <h1 className='xt-xl font-semibold'>{type === 'create' ? `Register new`: 'Update'} profile</h1>
        <span className='text-xs font-semibold'>Authentication Information</span>
        <div className="flex flex-wrap gap-8">
          <FormInputField label='Username' type='text' register={register} name='username' error={errors.username} defaultValue={data?.email} inputProps={{placeholder: 'Username', disabled: true}}/>
          <FormInputField label='Email' type='email' register={register} name='email' error={errors.email} defaultValue={data?.email} inputProps={{placeholder: 'email', disabled: true}}/>
          {/* //TODO: Add password validation if he user is not logged as social authentication */}
          {/* <FormInputField label='Password' type='password' register={register} name='password' error={errors.password} defaultValue={data?.password} inputProps={{placeholder: 'password'}}/> */}
        </div>
        
        {data?.socialLogin?.isSocialLogin ? null: (
          <>
          <span className='text-xs font-semibold'>Password</span>
          <div className="flex flex-wrap gap-8">
            <FormInputField label='Old Password' type='text' register={register} name='username' error={errors.username} defaultValue={data?.email} inputProps={{placeholder: 'Username', disabled: true}}/>
            <FormInputField label='New Password' type='email' register={register} name='email' error={errors.email} defaultValue={data?.email} inputProps={{placeholder: 'email', disabled: true}}/>
            <FormInputField label='Confirm password' type='email' register={register} name='email' error={errors.email} defaultValue={data?.email} inputProps={{placeholder: 'email', disabled: true}}/>
            {/* //TODO: Add password validation if he user is not logged as social authentication */}
            {/* <FormInputField label='Password' type='password' register={register} name='password' error={errors.password} defaultValue={data?.password} inputProps={{placeholder: 'password'}}/> */}
          </div>
          </>
        )}
       
        {/* PERSONAL INFORMATION */}
        <span className='text-xs font-semibold'>Personal Information</span>
        <div className="flex flex-wrap gap-8 items-center">
          <FormInputField label='First Name' type='text' register={register} name='firstName' error={errors.firstName} defaultValue={data?.firstName} inputProps={{placeholder: 'First Name'}}/>
          <FormInputField label='Last Name' type='text' register={register} name='lastName' error={errors.lastName} defaultValue={data?.lastName} inputProps={{placeholder: 'Last Name'}}/>
          
          {/* <div className='flex flex-col gap-2 md:w-1/4'>
              <label htmlFor={data?.sex} className='text-xs text-gray-500'>Gender</label>
              <select {...register("sex")} defaultValue={data?.sex} className='ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full'>
                <option value="--">--Select--</option>
                <option value="male">Male</option>
                <option value="female">Female</option>
              </select>
              {errors?.sex?.message && <p className='text-xs text-red-400 w-full'>{errors?.sex?.message}</p>}
          </div> */}
          <div className='flex flex-col gap-2 md:w-1/4 items-center justify-center'>
            <label className='flex gap-4 items-center align-middle justify-center cursor-pointer relative group' onClick={handleImageClick}>
                <img src={photoName ?? avatar} alt='' width={24} height={24} className='rounded-full w-14 h-14' title='Upload image' />
                {/* <span className='whitespace-nowrap'>{photoName}</span> */}
                <div className='absolute inset-0 flex items-center justify-center bg-black bg-opacity-10 rounded-full opacity-0 group-hover:opacity-100 transition-opacity duration-300'>
                    <img src={camera} alt='Overlay' className='w-8 h-8 rounded-full brightness-200' title={data?.picture ? 'Upload new image.': 'Upload image'} />
                </div>
            </label>
            <input type='file' {...register("img")} onChange={handleImageChange} className='hidden' ref={fileInputRef} />
            {errors?.img?.message && (<p className='text-xs text-red-400 w-full text-center'>{errors?.img?.message}</p>)}
        </div>
        </div>
        <button className='border-none rounded-md  px-4 py-2 text-white bg-onaxBlue font-semibold'>{type === 'update' ? "Update" : "Create"}</button>
    </form>
    </>
  )
}

export default ProfileForm