
import * as z from 'zod';

export const schema = z.object({
    username: z.string()
                .min(3, { message: 'Username must be at least 3 characters long.' })
                .max(20, { message: 'Username must be at most 20 characters long.' }),
    email: z.string().min(10).email({message: "Valid email is required"}),
    password: z.string().min(8, { message: 'Password must be at least 8 characters long.' }),
    firstName: z.string().min(3, { message: 'First name must be at least 3 characters long.' }),
    lastName: z.string().min(3, { message: 'Last name must be at least 3 characters long.' }),
    phone: z.string().min(3, { message: 'Phone is required.' }),
    address: z.string().min(3, { message: 'Address is required.' }),
    bloodType: z.string().min(2, { message: 'Blood type is required.' }),
    birthday: z.string({ message: 'Birth date is required.' }).refine(date => date > new Date().toISOString().split('T')[0], { message: 'Birth date is required.' }),
    sex: z.enum(["male","female","unspecified"], {message: 'Gender is required.'}),
    dateEmployed: z.date({ message: 'Date employed is required.' }).default(() => new Date()),
    img: z.instanceof(File, {message: 'Image is required.'}),

    //Subjects
    name: z.string().min(3, { message: 'Subject name must be at least 3 characters long.' }),
    teachers: z.array(z.string()).nonempty({ message: 'Teacher is required.' }),

    //Classes
    capacity: z.number().int().positive({ message: 'Capacity is required.' }),
    grade: z.number().int().positive({ message: 'Grade is required.' }),
    supervisor: z.string().min(3, { message: 'Supervisor is required.' }),

    //Lessons
    subject: z.string().min(3, { message: 'Subject is required.' }),
    class: z.string().min(1, { message: 'Class is required.' }),
    teacher: z.string().min(3, { message: 'Teacher is required.' }),

    //Exam
    date: z.string({ message: 'Date is required.' }).refine(date => date > new Date().toISOString().split('T')[0], { message: 'Date must be in the future.' }),

    //Assignment
    dueDate: z.string({ message: 'Due date is required.' }).refine(date => date >new Date().toISOString().split('T')[0], { message: 'Due date must be in the future.' }),

    //Results
    student: z.string().min(3, { message: 'Student is required.' }),
    type: z.enum(["exam", "assignment"], {message: 'Type is required and must be "exam" or "assignment".'}),
    score: z.string().min(1,{ message: 'Score is required.' }),

    //Events
    title: z.string().min(3, { message: 'Title is required.' }),
    startTime: z.string({ message: 'Start time is required.' }).refine(date => date >new Date().toISOString().split('T')[0], { message: 'Start time must be in the future.' }),
    endTime: z.string({ message: 'End time is required.' }).refine(date => date >new Date().toISOString().split('T')[0], { message: 'End time must be in the future.' }),

  });

  export type RegInput = z.infer<typeof schema>;