package com.lims.mobile.utils

import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import com.lims.mobile.dataStore
import com.lims.mobile.model.UserInfo
import com.google.gson.Gson
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

class PreferencesManager(private val context: Context) {
    
    private val gson = Gson()
    
    companion object {
        private val TOKEN_KEY = stringPreferencesKey("token")
        private val USER_KEY = stringPreferencesKey("user")
        private val SERVER_URL_KEY = stringPreferencesKey("server_url")
        
        // Default URLs
        const val DEV_EMULATOR_URL = "http://10.0.2.2:5047/api/v1/"
        const val DEV_DEVICE_URL = "http://192.168.1.100:5047/api/v1/"
        const val PROD_URL = "https://your-api.com/api/v1/"
    }
    
    // Token
    val token: Flow<String?> = context.dataStore.data.map { preferences ->
        preferences[TOKEN_KEY]
    }
    
    suspend fun saveToken(token: String) {
        context.dataStore.edit { preferences ->
            preferences[TOKEN_KEY] = token
        }
    }
    
    // User
    val user: Flow<UserInfo?> = context.dataStore.data.map { preferences ->
        preferences[USER_KEY]?.let { json ->
            gson.fromJson(json, UserInfo::class.java)
        }
    }
    
    suspend fun saveUser(user: UserInfo) {
        context.dataStore.edit { preferences ->
            preferences[USER_KEY] = gson.toJson(user)
        }
    }
    
    // Server URL
    val serverUrl: Flow<String> = context.dataStore.data.map { preferences ->
        preferences[SERVER_URL_KEY] ?: getDefaultServerUrl()
    }
    
    suspend fun saveServerUrl(url: String) {
        context.dataStore.edit { preferences ->
            preferences[SERVER_URL_KEY] = url
        }
    }
    
    fun getDefaultServerUrl(): String {
        return if (NetworkUtils.isEmulator()) DEV_EMULATOR_URL else DEV_DEVICE_URL
    }
    
    // Clear all
    suspend fun clear() {
        context.dataStore.edit { it.clear() }
    }
}