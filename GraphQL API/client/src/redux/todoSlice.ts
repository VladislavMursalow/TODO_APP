import {createAction, createSlice, PayloadAction} from "@reduxjs/toolkit";
import {TodoType} from "./types/models";
import {TodosCreateInputType, TodoUpdateInputType} from "../GraphQl/mutations";

interface TodoSlice {
    todos: Array<TodoType>,
    editTodo: TodoType | null | undefined,
    fetchingTodosIds: Array<number>
    createTodoId: number,
    selectedCategoryId: number | null
}

const initialState: TodoSlice = {
    todos: [],
    fetchingTodosIds: [],
    editTodo: null,
    createTodoId: 0,
    selectedCategoryId: null
}

export const todoSlice = createSlice({
    name: 'todo',
    initialState,
    reducers: {
        addTodos: (state, action: PayloadAction<TodoType[]>) => {
            state.todos = action.payload
        },
        addTodo: (state, action: PayloadAction<TodoType>) => {
            let todo = action.payload
            state.createTodoId += state.todos.at(-1)?.id as number
            todo.id = state.createTodoId + 1
            state.todos.push(todo)
        },
        addFetchingTodosId: (state, action: PayloadAction<number>) => {
            state.fetchingTodosIds.push(action.payload)
        },
        removeFetchingTodosId: (state, action: PayloadAction<number>) => {
            state.fetchingTodosIds = state.fetchingTodosIds.filter(id => id !== action.payload)
        },
        toggleIsDone: (state, action: PayloadAction<number>) => {
            const id = action.payload
            let todo = state.todos.find(e => e.id === id)
            if (todo) {
                todo.isDone = !todo.isDone
            }
        },
        removeTodo: (state, action: PayloadAction<number>) => {
            state.todos = state.todos.filter(todo => todo.id !== action.payload)
        },
        removeEditTodo: (state, action) => {
            state.editTodo = null
        },
        updateTodo: (state, action: PayloadAction<TodoType>) => {
            let todo = action.payload
            state.todos = state.todos.map(t => {
                if (t.id === todo.id) return {...t, ...todo}
                return t
            })
        },
        setSelectedCategoryId: (state, action: PayloadAction<number>) => {
            state.selectedCategoryId = action.payload
        },
        resetSelectedCategoryId: (state) => {
            state.selectedCategoryId = null
        },
    }
})

export default todoSlice.reducer

export const {
    addTodo, removeTodo,
    toggleIsDone,
    setSelectedCategoryId,
    resetSelectedCategoryId, addTodos,
    updateTodo,
    addFetchingTodosId, removeFetchingTodosId
} = todoSlice.actions

export const fetchTodosAsync = createAction("todo/fetchTodosAsync")
export const deleteTodoAsync = createAction<number>("todo/deleteTodoAsync")
export const updateTodoAsync = createAction<TodoUpdateInputType>("todo/updateTodoAsync")
export const createTodoAsync = createAction<TodosCreateInputType>("todo/createTodoAsync")
export const toggleIsDoneAsync = createAction<number>("todo/toggleIsDoneAsync")
