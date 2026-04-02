package com.lims.mobile.model

import com.google.gson.annotations.SerializedName

data class LoginRequest(
    val username: String,
    val password: String
)

data class LoginResponse(
    val code: Int,
    val message: String,
    val data: LoginData?
)

data class LoginData(
    val token: String,
    @SerializedName("tokenType") val tokenType: String,
    @SerializedName("expiresIn") val expiresIn: Long,
    val user: UserInfo
)

data class UserInfo(
    val id: String,
    val username: String,
    val role: String,
    @SerializedName("fullName") val fullName: String?
)

data class ApiResponse<T>(
    val code: Int,
    val message: String,
    val data: T?
)