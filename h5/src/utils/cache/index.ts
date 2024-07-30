import { createStorage, CreateStorageParams } from './storageCache'
import {
  cacheCipher,
  DEFAULT_CACHE_TIME,
  DEFAULT_PREFIX_KEY,
  enableStorageEncryption
} from '@/settings/encryptionSetting'

const options: Partial<CreateStorageParams> = {
  prefixKey: DEFAULT_PREFIX_KEY,
  key: cacheCipher.key,
  iv: cacheCipher.iv,
  hasEncrypt: enableStorageEncryption,
  timeout: DEFAULT_CACHE_TIME
}

export const storage = createStorage(options)

/**
 * 设置缓存
 * @param key
 * @param value
 * @param expire 多少秒之后过期(eg: 3600)
 */
export function setCache(key: string, value: any, expire?: number | null): void {
  storage.set(key, value, expire)
}

export function getCache<T = any>(key: string): T {
  return storage.get<T>(key)
}

export function removeCache(key: string): void {
  return storage.remove(key)
}

export function clearCache(): void {
  return storage.clear()
}
