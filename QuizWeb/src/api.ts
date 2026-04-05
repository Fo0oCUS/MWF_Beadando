import type {
  ApiProblem,
  AuthSession,
  CreateQuizRequest,
  LoginRequest,
  LoginResponse,
  QuizDetails,
  QuizSummary,
  RegisterRequest,
  SessionJoinResponse,
  SessionState,
} from './types'

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined)?.trim() || 'http://localhost:5044'

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
  authSession?: AuthSession | null,
): Promise<T> {
  const headers = new Headers(options.headers)
  if (!headers.has('Content-Type') && options.body) {
    headers.set('Content-Type', 'application/json')
  }
  if (authSession?.authToken) {
    headers.set('Authorization', `Bearer ${authSession.authToken}`)
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  })

  if (response.status === 204) {
    return undefined as T
  }

  const text = await response.text()
  const data = text ? (JSON.parse(text) as T | ApiProblem) : undefined

  if (!response.ok) {
    const problem = data as ApiProblem | undefined
    throw new ApiError(
      response.status,
      problem?.detail || problem?.title || problem?.message || 'A kérés sikertelen volt.',
    )
  }

  return data as T
}

export function registerUser(payload: RegisterRequest): Promise<void> {
  return apiRequest<void>('/users', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function loginUser(payload: LoginRequest): Promise<LoginResponse> {
  return apiRequest<LoginResponse>('/users/login', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function getMyQuizzes(authSession: AuthSession): Promise<QuizSummary[]> {
  return apiRequest<QuizSummary[]>('/quizzes/mine', {}, authSession)
}

export function createQuiz(payload: CreateQuizRequest, authSession: AuthSession): Promise<QuizDetails> {
  return apiRequest<QuizDetails>(
    '/quizzes',
    {
      method: 'POST',
      body: JSON.stringify(payload),
    },
    authSession,
  )
}

export function getQuizDetails(quizId: number, authSession: AuthSession): Promise<QuizDetails> {
  return apiRequest<QuizDetails>(`/quizzes/${quizId}`, {}, authSession)
}

export function createQuizSession(quizId: number, authSession: AuthSession): Promise<SessionState> {
  return apiRequest<SessionState>(
    '/quiz-sessions',
    {
      method: 'POST',
      body: JSON.stringify({ quizId }),
    },
    authSession,
  )
}

export function getLatestSessionForQuiz(quizId: number, authSession: AuthSession): Promise<SessionState> {
  return apiRequest<SessionState>(`/quiz-sessions/quiz/${quizId}/latest`, {}, authSession)
}

export function getHostSession(sessionId: number, authSession: AuthSession): Promise<SessionState> {
  return apiRequest<SessionState>(`/quiz-sessions/${sessionId}`, {}, authSession)
}

export function getParticipantSession(joinCode: string, participantId?: number): Promise<SessionState> {
  const query = participantId ? `?participantId=${participantId}` : ''
  return apiRequest<SessionState>(`/quiz-sessions/by-code/${joinCode.toUpperCase()}${query}`)
}

export function joinSession(joinCode: string, nickname: string): Promise<SessionJoinResponse> {
  return apiRequest<SessionJoinResponse>('/session-participants/join', {
    method: 'POST',
    body: JSON.stringify({
      joinCode: joinCode.toUpperCase(),
      nickname,
    }),
  })
}

export function submitParticipantAnswer(
  sessionParticipantId: number,
  answerOptionId: number,
  responseTimeMs: number,
): Promise<void> {
  return apiRequest<void>('/participant-answers', {
    method: 'POST',
    body: JSON.stringify({
      sessionParticipantId,
      answerOptionId,
      responseTimeMs,
    }),
  })
}

export function startSession(sessionId: number, authSession: AuthSession): Promise<SessionState> {
  return apiRequest<SessionState>(`/quiz-sessions/${sessionId}/start`, { method: 'PATCH' }, authSession)
}

export function closeQuestion(sessionId: number, authSession: AuthSession): Promise<SessionState> {
  return apiRequest<SessionState>(
    `/quiz-sessions/${sessionId}/close-question`,
    { method: 'PATCH' },
    authSession,
  )
}

export function nextQuestion(sessionId: number, authSession: AuthSession): Promise<SessionState> {
  return apiRequest<SessionState>(
    `/quiz-sessions/${sessionId}/next-question`,
    { method: 'PATCH' },
    authSession,
  )
}
