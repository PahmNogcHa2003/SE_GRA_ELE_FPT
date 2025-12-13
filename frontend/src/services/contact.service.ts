import type { CreateContactDTO } from '../types/contact'
import type { ApiResponse } from '../types/api'
import { httpUser } from './http'; 

const BASE_URL = '/contacts';

export const createContact = async (payload : CreateContactDTO) =>{
    const res = await httpUser.post<ApiResponse<CreateContactDTO>>(BASE_URL,payload);
    return res.data
}