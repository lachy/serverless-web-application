import { AxiosResponse } from 'axios';
import useAxios from './useAxios';
import { Todo } from '../model/todo';

const useTodosApi = () => {
  const {
    get, post, put, del,
  } = useAxios();
  return {
    getAllTodos: (): Promise<AxiosResponse<Todo>> => get('todos'),
    getTodo: (id: string): Promise<AxiosResponse<Todo>> => get(`todos/${id}`),
    updateTodo: (todo: Todo) => put('todos', todo),
    createTodo: (todo: Todo) => post('todos', todo),
    deleteTodo: (id: string) => del(`todos/${id}`),
  };
};

export default useTodosApi;
