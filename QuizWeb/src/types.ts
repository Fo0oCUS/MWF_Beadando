export interface AuthSession {
  userId: string
  authToken: string
  refreshToken: string
}

export interface ApiProblem {
  title?: string
  detail?: string
  status?: number
  message?: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  name: string
  email: string
  password: string
}

export interface LoginResponse {
  userId: string
  authToken: string
  refreshToken: string
}

export interface QuizSummary {
  id: number
  title: string
  isPublished: boolean
  questionCount: number
}

export interface QuizAnswerOption {
  id: number
  text: string
  orderIndex: number
  isCorrect: boolean
}

export interface QuizQuestion {
  id: number
  text: string
  orderIndex: number
  timeLimitSeconds: number
  answerOptions: QuizAnswerOption[]
}

export interface QuizDetails {
  id: number
  title: string
  isPublished: boolean
  questions: QuizQuestion[]
}

export interface CreateQuizRequest {
  title: string
  isPublished: boolean
  questions: Array<{
    text: string
    orderIndex: number
    timeLimitSeconds: number
    answerOptions: Array<{
      text: string
      orderIndex: number
      isCorrect: boolean
    }>
  }>
}

export interface SessionJoinResponse {
  participantId: number
  sessionId: number
  joinCode: string
  nickname: string
  totalScore: number
}

export interface SessionAnswerOptionView {
  id: number
  text: string
  orderIndex: number
}

export interface SessionQuestionView {
  id: number
  text: string
  orderIndex: number
  timeLimitSeconds: number
  answerOptions: SessionAnswerOptionView[]
}

export interface SessionAnswerResultItem {
  answerOptionId: number
  text: string
  isCorrect: boolean
}

export interface SessionQuestionResults {
  questionId: number
  answers: SessionAnswerResultItem[]
}

export interface ParticipantSessionView {
  participantId: number
  nickname: string
  totalScore: number
  hasAnsweredCurrentQuestion: boolean
  selectedAnswerOptionId: number | null
}

export interface SessionState {
  sessionId: number
  quizId: number
  quizTitle: string
  joinCode: string
  stage: 'lobby' | 'question-open' | 'question-closed' | 'finished'
  currentQuestionIndex: number
  totalQuestionCount: number
  participantCount: number
  canJoin: boolean
  canAnswer: boolean
  hasStarted: boolean
  isFinished: boolean
  viewer: ParticipantSessionView | null
  currentQuestion: SessionQuestionView | null
  currentQuestionResults: SessionQuestionResults | null
}

export interface StoredParticipant {
  participantId: number
  nickname: string
}
