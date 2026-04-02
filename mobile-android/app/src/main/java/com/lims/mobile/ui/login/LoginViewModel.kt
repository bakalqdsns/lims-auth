package com.lims.mobile.ui.login

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.lims.mobile.data.AuthRepository
import com.lims.mobile.model.UserInfo
import com.lims.mobile.network.AuthApi
import com.lims.mobile.utils.PreferencesManager
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch

sealed class LoginUiState {
    object Idle : LoginUiState()
    object Loading : LoginUiState()
    data class Success(val user: UserInfo) : LoginUiState()
    data class Error(val message: String) : LoginUiState()
}

class LoginViewModel(
    private val prefs: PreferencesManager,
    private val isEmulator: Boolean
) : ViewModel() {

    private val _uiState = MutableStateFlow<LoginUiState>(LoginUiState.Idle)
    val uiState: StateFlow<LoginUiState> = _uiState.asStateFlow()

    private val _currentServerUrl = MutableStateFlow("")
    val currentServerUrl: StateFlow<String> = _currentServerUrl.asStateFlow()

    val isLoggedIn: StateFlow<Boolean> = prefs.token.map { it != null }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(), false)

    private lateinit var repository: AuthRepository

    init {
        viewModelScope.launch {
            prefs.serverUrl.collect { url ->
                _currentServerUrl.value = url
                initRepository(url)
            }
        }
    }

    private fun initRepository(baseUrl: String) {
        val api = AuthApi.create(baseUrl)
        repository = AuthRepository(api, prefs)
    }

    fun login(username: String, password: String) {
        viewModelScope.launch {
            _uiState.value = LoginUiState.Loading
            
            repository.login(username, password)
                .onSuccess { data ->
                    _uiState.value = LoginUiState.Success(data.user)
                }
                .onFailure { error ->
                    val message = when {
                        error.message?.contains("Unable to resolve host") == true ->
                            "无法连接到服务器，请检查网络"
                        error.message?.contains("timeout") == true ->
                            "连接超时，请检查服务器地址"
                        else -> error.message ?: "登录失败"
                    }
                    _uiState.value = LoginUiState.Error(message)
                }
        }
    }

    fun setServerUrl(url: String) {
        viewModelScope.launch {
            prefs.saveServerUrl(url)
        }
    }

    class Factory(
        private val prefs: PreferencesManager,
        private val isEmulator: Boolean
    ) : ViewModelProvider.Factory {
        @Suppress("UNCHECKED_CAST")
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            return LoginViewModel(prefs, isEmulator) as T
        }
    }
}