// import { TFunction } from "i18next";
// import { ApplicationState } from "./initial"

// export const getUserFromState = ({ users, allUsers }: ApplicationState) => (userId: any, includeVirtual: boolean = false) =>
//     (includeVirtual ? allUsers : users).find(u =>
//         typeof userId === 'number'
//             ? u.oldId === userId
//             : u.id == userId
//     );

// export const getDepartmentFromState = ({ departments }: ApplicationState) => (departmentId: number) =>
//     departments.find(d =>
//         d.id === departmentId
//     );

// export const createUserNameDisplay = (state: ApplicationState, t: TFunction) => (userId: string) => {
//     const user = getUserFromState(state)(userId);
//     return `${user?.name ?? ""} ${user?.active === false ? `(${t('inactive')})` : ""}`
// }

export const apa = '23';