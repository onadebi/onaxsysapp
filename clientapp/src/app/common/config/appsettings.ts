/**
 * Application settings configuration.
 */
const appsettings = {
    appName: 'OnaxApp',
    appKey: 'OnaxAppKey',
    appUrl: window.location.origin, //'http://localhost:3500',
    // appUrl: 'https://coursetakeoff.live',
    env: 'development',
    baseUrls: {
        baseUrl: 'http://localhost:5050', 
        // baseUrl: `${window.location.origin}/api`,
    },
    Auth:{
        Clerk:{
            PUBLISHABLE_KEY: import.meta.env.VITE_CLERK_PUBLISHABLE_KEY,
        }
    },
    FileUploadedConstraints:{
        images: ['image/jpeg', 'image/png', 'image/gif','video/mp4','video/webm','video/avi','video/mpeg','video/ogg','video/x-ms-wmv','video/x-flv','video/3gpp','video/3gpp2','video/x-matroska'],
    },
    Blog:{
        Posts:{
            Validations:{
                titleMinCharactersLength : 10,
                contentMinCharactersLength: 100,
            }
        }
    },
    functions: {
        /**
         * Converts the first character of the text to uppercase.
         * @param text - The text to convert.
         * @returns The text with the first character in uppercase.
         */
        ToSentenceUpper(text: string) {
            return text && text.length > 1 ? (text[0].toUpperCase() + text.slice(1)): text;
        },
        /**
         * Formats a number with commas as thousand separators.
         * @param x - The number to format.
         * @returns The formatted number as a string.
         */
        NumberCommaFormat: (x: number) => {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        },
        /**
         * Calculates a future date based on the number of days from today.
         * @param days - The number of days to add to the current date. Default is 1.
         * @returns The future date as a string in YYYY-MM-DD format.
         */
        FuturDate: (days: number = 1) => {
            return new Date(Date.now() + (days * 24 * 60 * 60 * 1000)).toISOString().split('T')[0];
        },
        BooleanToYesNo: (status: boolean) => {
            return status ? 'Yes' : 'No';
        },
    },
    externalServices: {
        YoutubeUrlByVideoId: (videoId: string) => {
            return `https://www.youtube.com/watch?v=${videoId}`;
        },
        GoogleAuth:{
            clientId: import.meta.env.VITE_GOOGLE_CLIENT_ID,
        }
    },
    public:{
        assets:{
            images:{
                finding: '/assets/images/search.gif',
                rocket: "/assets/images/rocket-loader.gif",
                folder: "/assets/images/folder.gif",
                course_banner_default: "/assets/images/course_img_default.png",
            }
        }
    },
    token: {
        authName: 'xray-token',
        authToken: 'onx_token'
    },
};
export const nameOfFactory = <T>()=> (name: keyof T)=> name;

export default appsettings;