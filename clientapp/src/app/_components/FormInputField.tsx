import React from 'react';
import { FieldError, UseFormRegister } from 'react-hook-form';
import { RegInput } from './forms/schema';


type IProps = {
    label: string;
    type?: string;
    register: UseFormRegister<RegInput>; //UseFormRegister<FieldValues>;
    name: keyof RegInput;
    error?: FieldError;
    defaultValue?: string| number;
    inputProps?: React.InputHTMLAttributes<HTMLInputElement>;
}

const FormInputField: React.FC<IProps> = ({label, type,inputProps,name,defaultValue,register,error}) => {

  return (
    <>
        <div className='flex flex-col gap-2 md:w-1/4'>
            <label htmlFor={label} className='text-xs text-gray-500'>{label}</label>
            <input type={type} {...register(name)} defaultValue={defaultValue} className='ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full' {...inputProps}/>
            {error?.message && <p className='text-xs text-red-400 w-full'>{error?.message.toString()}</p>}
        </div>
    </>
  )
}

export default FormInputField