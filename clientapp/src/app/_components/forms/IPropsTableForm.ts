export interface IPropsTableForm<T> {
    type: "create" | "update"| "delete";
    data?: T;
    id?: string;
}