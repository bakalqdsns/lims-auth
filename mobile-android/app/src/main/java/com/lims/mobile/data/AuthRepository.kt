package com.lims.mobile.data

import com.lims.mobile.model.LoginData
import com.lims.mobile.model.LoginRequest
import com.lims.mobile.model.UserInfo
import com.lims.mobile.network.AuthApi
import com.lims.mobile.utils.PreferencesManager

class AuthRepository(
    private val api: AuthApi,
    private val prefs: PreferencesManager
) {
    
    suspend fun login(username: String, password: String): Result<LoginData> {
        return try {
            val response = api.login(LoginRequest(username, password))
            
            if (response.isSuccessful) {
                val body = response.body()
                if (body?.code == 200 && body.data != null) {
                    // Save to preferences
                    prefs.saveToken(body.data.token)
                    prefs.saveUser(body.data.user)
                    Result.success(body.data)
                } else {
                    Result.failure(Exception(body?.message ?: "登录失败"))
                }
            } else {
                Result.failure(Exception("服务器错误: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getCurrentUser(token: String): Result<UserInfo> {
        return try {
            val response = api.getCurrentUser("Bearer $token")
            
            if (response.isSuccessful) {
                val body = response.body()
                if (body?.code == 200 && body.data != null) {
                    Result.success(body.data)
                } else {
                    Result.failure(Exception(body?.message ?: "获取用户信息失败"))
                }
            } else {
                Result.failure(Exception("服务器错误: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun logout() {
        prefs.clear()
    }
    
    fun getTokenFlow() = prefs.token
    fun getUserFlow() = prefs.user
}