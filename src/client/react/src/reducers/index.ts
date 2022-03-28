import { combineReducers } from 'redux';
import errorReducer from './errors';

const reducers = combineReducers({
  errorReducer,
});

export default reducers;
export type AppState = ReturnType<typeof reducers>;
