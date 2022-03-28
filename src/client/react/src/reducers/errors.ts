import { AnyAction } from 'redux';
import { API_ERROR } from '../actions/shared';

interface ErrorState {
  error: string | null;
}

const initialState: ErrorState = {
  error: null,
};

export default function errorReducer(state = initialState, action: AnyAction) {
  const { type, payload } = action;
  switch (type) {
    case API_ERROR:
      return {
        ...state,
        error: payload,
      };

    default:
      return state;
  }
}
