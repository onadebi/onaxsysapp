export interface CommonEntity {
    createdAt: Date;
    updatedAt?: Date | null;
    guid: string;
    isActive: boolean;
    isDeleted: boolean;
}