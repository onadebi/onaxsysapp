import { UserLoginResponse } from "../models/UserLoginResponse";

export type Role = keyof typeof ROLES;
export type Permissions = (typeof ROLES)[Role][number];


const ROLES = {
    admin:[
        "view:dashboard"
    ],
    user:[
        "view:dashboard", "view:profile","edit:profile"
    ],
    guest:["view:home"]
}

export function hasPermission(user: UserLoginResponse, permission: Permissions[]): boolean {
    const userPerms = user.roles.map((role)=> ROLES[role as Role] as readonly Permissions[]);
    //UserPerms flattened with duplicates removed
    const userPermsFlattened = [...new Set(userPerms.flat())] as readonly Permissions[];
    const userHasPerm =  userPermsFlattened.some((perm) => permission.includes(perm));
    return userHasPerm;
}


//#region Explanation to use only readonly
// // Without readonly
// const regularPerms = ROLES["admin" as Role];
// regularPerms.push("newPermission"); // ✅ Allowed
// regularPerms[0] = "modifiedPermission"; // ✅ Allowed

// // With readonly
// const readonlyPerms = ROLES["admin" as Role] as readonly Permissions[];
// readonlyPerms.push("newPermission"); // ❌ Error: Property 'push' does not exist on type 'readonly Permissions[]'
// readonlyPerms[0] = "modifiedPermission"; // ❌ Error: Index signature in type 'readonly Permissions[]' only permits reading
//#endregion
