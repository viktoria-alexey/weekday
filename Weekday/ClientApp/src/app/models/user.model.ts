export class User {
    constructor(id?: string, userName?: string, email?: string, jobTitle?: string, phoneNumber?: string, roleIds?: string[]) {

        this.id = id;
        this.userName = userName;
        this.email = email;
        this.jobTitle = jobTitle;
        this.phoneNumber = phoneNumber;
        this.roleIds = roleIds;
    }

    public id: string;
    public userName: string;
    public fullName: string;
    public email: string;
    public jobTitle: string;
    public phoneNumber: string;
    public roleIds: string[];
}