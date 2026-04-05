import type { AuthSession, StoredParticipant } from './types'

const AUTH_STORAGE_KEY = 'quizweb.auth-session'
const PARTICIPANT_STORAGE_PREFIX = 'quizweb.participant.'

export function loadAuthSession(): AuthSession | null {
  const rawValue = localStorage.getItem(AUTH_STORAGE_KEY)
  if (!rawValue) {
    return null
  }

  try {
    return JSON.parse(rawValue) as AuthSession
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY)
    return null
  }
}

export function saveAuthSession(session: AuthSession): void {
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session))
}

export function clearAuthSession(): void {
  localStorage.removeItem(AUTH_STORAGE_KEY)
}

export function loadStoredParticipant(joinCode: string): StoredParticipant | null {
  const rawValue = sessionStorage.getItem(PARTICIPANT_STORAGE_PREFIX + joinCode.toUpperCase())
  if (!rawValue) {
    return null
  }

  try {
    return JSON.parse(rawValue) as StoredParticipant
  } catch {
    sessionStorage.removeItem(PARTICIPANT_STORAGE_PREFIX + joinCode.toUpperCase())
    return null
  }
}

export function saveStoredParticipant(joinCode: string, participant: StoredParticipant): void {
  sessionStorage.setItem(
    PARTICIPANT_STORAGE_PREFIX + joinCode.toUpperCase(),
    JSON.stringify(participant),
  )
}

export function clearStoredParticipant(joinCode: string): void {
  sessionStorage.removeItem(PARTICIPANT_STORAGE_PREFIX + joinCode.toUpperCase())
}
